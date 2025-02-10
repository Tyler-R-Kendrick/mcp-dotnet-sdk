using Abstractions.Models;
using StreamJsonRpc;

namespace Client;

public class ServerJsonRpcTransport(JsonRpc jsonRpc) : ITransport
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        jsonRpc.StartListening();
        await Task.CompletedTask; // Placeholder for actual start logic
    }

    public async Task<TResult> SendAsync<TResult>(
        IMessage message,
        CancellationToken cancellationToken)
    {
        var response = await jsonRpc.InvokeWithCancellationAsync<TResult>(
            message.Method,
            [message],
            cancellationToken);
        OnMessage?.Invoke(message);
        return response;
    }

    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        jsonRpc.Dispose();
        OnClose?.Invoke();
        await Task.CompletedTask; // Placeholder for actual close logic
    }

    public event Action OnClose = delegate { };
    public event Action<Exception> OnError = delegate { };
    public event Action<IMessage> OnMessage = delegate { };
}
