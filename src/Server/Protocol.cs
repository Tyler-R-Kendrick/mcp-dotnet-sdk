using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abstractions.Models;

namespace Server
{
    public abstract class Protocol<SendRequestT, SendNotificationT, SendResultT>
    {
        private ITransport _transport;
        private int _requestMessageId = 0;
        private readonly Dictionary<string, Func<JSONRPCRequest, RequestHandlerExtra, Task<SendResultT>>> _requestHandlers = new();
        private readonly Dictionary<RequestId, AbortController> _requestHandlerAbortControllers = new();
        private readonly Dictionary<string, Func<JSONRPCNotification, Task>> _notificationHandlers = new();
        private readonly Dictionary<int, Action<JSONRPCResponse>> _responseHandlers = new();
        private readonly Dictionary<int, ProgressCallback> _progressHandlers = new();

        public event Action OnClose;
        public event Action<Exception> OnError;

        public Func<Request, Task<SendResultT>> FallbackRequestHandler;
        public Func<Notification, Task> FallbackNotificationHandler;

        protected Protocol(ProtocolOptions options = null)
        {
            SetNotificationHandler(CancelledNotificationSchema, notification =>
            {
                var controller = _requestHandlerAbortControllers.GetValueOrDefault(notification.Params.RequestId);
                controller?.Abort(notification.Params.Reason);
                return Task.CompletedTask;
            });

            SetNotificationHandler(ProgressNotificationSchema, notification =>
            {
                OnProgress(notification as ProgressNotification);
                return Task.CompletedTask;
            });

            SetRequestHandler(PingRequestSchema, _ => Task.FromResult((SendResultT)Activator.CreateInstance(typeof(SendResultT))));
        }

        public async Task Connect(ITransport transport)
        {
            _transport = transport;
            _transport.OnClose += () => OnClose?.Invoke();
            _transport.OnError += error => OnError?.Invoke(error);
            _transport.OnMessage += message =>
            {
                if (message is JSONRPCResponse response)
                {
                    OnResponse(response);
                }
                else if (message is JSONRPCRequest request)
                {
                    OnRequest(request);
                }
                else if (message is JSONRPCNotification notification)
                {
                    OnNotification(notification);
                }
            };

            await _transport.StartAsync();
        }

        private void OnClose()
        {
            var responseHandlers = new Dictionary<int, Action<JSONRPCResponse>>(_responseHandlers);
            _responseHandlers.Clear();
            _progressHandlers.Clear();
            _transport = null;
            OnClose?.Invoke();

            var error = new McpError(ErrorCode.ConnectionClosed, "Connection closed");
            foreach (var handler in responseHandlers.Values)
            {
                handler(new JSONRPCResponse { Error = error });
            }
        }

        private void OnError(Exception error)
        {
            OnError?.Invoke(error);
        }

        private void OnNotification(JSONRPCNotification notification)
        {
            var handler = _notificationHandlers.GetValueOrDefault(notification.Method) ?? FallbackNotificationHandler;
            if (handler == null) return;

            Task.Run(async () =>
            {
                try
                {
                    await handler(notification);
                }
                catch (Exception ex)
                {
                    OnError(new Exception($"Uncaught error in notification handler: {ex}"));
                }
            });
        }

        private void OnRequest(JSONRPCRequest request)
        {
            var handler = _requestHandlers.GetValueOrDefault(request.Method) ?? FallbackRequestHandler;
            if (handler == null)
            {
                _transport.SendAsync(new JSONRPCResponse
                {
                    Id = request.Id,
                    Error = new JSONRPCError
                    {
                        Code = ErrorCode.MethodNotFound,
                        Message = "Method not found"
                    }
                });
                return;
            }

            var abortController = new AbortController();
            _requestHandlerAbortControllers[request.Id] = abortController;

            Task.Run(async () =>
            {
                try
                {
                    var result = await handler(request, new RequestHandlerExtra { Signal = abortController.Signal });
                    if (!abortController.Signal.IsCancellationRequested)
                    {
                        await _transport.SendAsync(new JSONRPCResponse { Id = request.Id, Result = result });
                    }
                }
                catch (Exception ex)
                {
                    if (!abortController.Signal.IsCancellationRequested)
                    {
                        await _transport.SendAsync(new JSONRPCResponse
                        {
                            Id = request.Id,
                            Error = new JSONRPCError
                            {
                                Code = ErrorCode.InternalError,
                                Message = ex.Message
                            }
                        });
                    }
                }
                finally
                {
                    _requestHandlerAbortControllers.Remove(request.Id);
                }
            });
        }

        private void OnProgress(ProgressNotification notification)
        {
            if (_progressHandlers.TryGetValue(notification.Params.ProgressToken, out var handler))
            {
                handler(notification.Params);
            }
            else
            {
                OnError(new Exception($"Received a progress notification for an unknown token: {notification}"));
            }
        }

        private void OnResponse(JSONRPCResponse response)
        {
            if (_responseHandlers.TryGetValue(response.Id, out var handler))
            {
                _responseHandlers.Remove(response.Id);
                _progressHandlers.Remove(response.Id);
                handler(response);
            }
            else
            {
                OnError(new Exception($"Received a response for an unknown message ID: {response}"));
            }
        }

        public async Task CloseAsync()
        {
            await _transport?.CloseAsync();
        }

        protected abstract void AssertCapabilityForMethod(string method);
        protected abstract void AssertNotificationCapability(string method);
        protected abstract void AssertRequestHandlerCapability(string method);

        public async Task<TResult> RequestAsync<TRequest, TResult>(
            TRequest request, RequestOptions options = null)
        {
            if (_transport == null) throw new InvalidOperationException("Not connected");

            if (_options?.EnforceStrictCapabilities == true)
            {
                AssertCapabilityForMethod(request.Method);
            }

            options?.Signal?.ThrowIfCancellationRequested();

            var messageId = _requestMessageId++;
            JSONRPCRequest jsonrpcRequest = new
            {
                Method = request.Method,
                Params = request.Params,
                Id = messageId
            };

            if (options?.OnProgress != null)
            {
                _progressHandlers[messageId] = options.OnProgress;
                jsonrpcRequest.Params._meta = new { ProgressToken = messageId };
            }

            var tcs = new TaskCompletionSource<T>();
            _responseHandlers[messageId] = response =>
            {
                if (options?.Signal?.IsCancellationRequested == true)
                {
                    tcs.SetCanceled();
                    return;
                }

                if (response.Error != null)
                {
                    tcs.SetException(new McpError(response.Error.Code, response.Error.Message, response.Error.Data));
                }
                else
                {
                    try
                    {
                        var result = resultSchema.Parse(response.Result);
                        tcs.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }
            };

            var timeout = options?.Timeout ?? ProtocolOptions.DEFAULT_REQUEST_TIMEOUT_MSEC;
            var timeoutCts = new CancellationTokenSource(timeout);
            timeoutCts.Token.Register(() => tcs.TrySetException(new McpError(ErrorCode.RequestTimeout, "Request timed out", new { Timeout = timeout })));

            options?.Signal?.Register(() => tcs.TrySetCanceled());

            await _transport.SendAsync(jsonrpcRequest);

            return await tcs.Task;
        }

        public async Task NotificationAsync(SendNotificationT notification)
        {
            if (_transport == null) throw new InvalidOperationException("Not connected");

            AssertNotificationCapability(notification.Method);

            var jsonrpcNotification = new JSONRPCNotification
            {
                Method = notification.Method,
                Params = notification.Params
            };

            await _transport.SendAsync(jsonrpcNotification);
        }

        public void SetRequestHandler<T>(ZodObject<T> requestSchema, Func<ZodObject<T>, RequestHandlerExtra, Task<SendResultT>> handler)
        {
            var method = requestSchema.Shape.Method.Value;
            AssertRequestHandlerCapability(method);
            _requestHandlers[method] = (request, extra) => handler(requestSchema.Parse(request), extra);
        }

        public void RemoveRequestHandler(string method)
        {
            _requestHandlers.Remove(method);
        }

        public void SetNotificationHandler<T>(ZodObject<T> notificationSchema, Func<ZodObject<T>, Task> handler)
        {
            _notificationHandlers[notificationSchema.Shape.Method.Value] = notification => handler(notificationSchema.Parse(notification));
        }

        public void RemoveNotificationHandler(string method)
        {
            _notificationHandlers.Remove(method);
        }
    }

    public delegate void ProgressCallback(Progress progress);

    public class ProtocolOptions
    {
        public bool EnforceStrictCapabilities { get; set; } = false;
        public const int DEFAULT_REQUEST_TIMEOUT_MSEC = 60000;
    }

    public class RequestOptions
    {
        public ProgressCallback OnProgress { get; set; }
        public CancellationToken Signal { get; set; }
        public int Timeout { get; set; } = ProtocolOptions.DEFAULT_REQUEST_TIMEOUT_MSEC;
    }

    public class RequestHandlerExtra
    {
        public CancellationToken Signal { get; set; }
    }

    public class JSONRPCRequest
    {
        public string Method { get; set; }
        public object Params { get; set; }
        public int Id { get; set; }
    }

    public class JSONRPCNotification
    {
        public string Method { get; set; }
        public object Params { get; set; }
    }

    public class JSONRPCResponse
    {
        public int Id { get; set; }
        public object Result { get; set; }
        public JSONRPCError Error { get; set; }
    }

    public class JSONRPCError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class McpError : Exception
    {
        public int Code { get; }
        public object Data { get; }

        public McpError(int code, string message, object data = null) : base(message)
        {
            Code = code;
            Data = data;
        }
    }

    public class ErrorCode
    {
        public const int ConnectionClosed = -32000;
        public const int MethodNotFound = -32601;
        public const int InternalError = -32603;
        public const int RequestTimeout = -32001;
    }

    public class ProgressNotification
    {
        public Progress Params { get; set; }
    }

    public class Progress
    {
        public int ProgressToken { get; set; }
    }

    public class AbortController
    {
        private readonly CancellationTokenSource _cts = new();

        public CancellationToken Signal => _cts.Token;

        public void Abort(string reason = null)
        {
            _cts.Cancel();
        }
    }
}
