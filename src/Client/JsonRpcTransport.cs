using System;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Models;
using StreamJsonRpc;

namespace Client;

public class JsonRpcTransport : ITransport
{
    private readonly JsonRpc _jsonRpc;

    public JsonRpcTransport(JsonRpc jsonRpc)
    {
        _jsonRpc = jsonRpc;
        _jsonRpc.Disconnected += (sender, e) => OnClose?.Invoke();
        _jsonRpc.Faulted += (sender, e) => OnError?.Invoke(e);
        _jsonRpc.MessageReceived += (sender, e) => OnMessage?.Invoke((IRequest)e.Message);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask; // Placeholder for actual start logic
    }

    public async Task SendAsync(IRequest message, CancellationToken cancellationToken)
    {
        await _jsonRpc.NotifyAsync(message.Method, message, cancellationToken);
    }

    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        await _jsonRpc.Completion;
        OnClose?.Invoke();
    }

    public event Action OnClose = delegate { };
    public event Action<Exception> OnError = delegate { };
    public event Action<IRequest> OnMessage = delegate { };
}
