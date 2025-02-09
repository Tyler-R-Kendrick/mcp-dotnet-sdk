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

    public async Task SendAsync(IRequest message, CancellationToken cancellationToken)
    {
        await jsonRpc.NotifyAsync(message.Method, message, cancellationToken);
        OnMessage?.Invoke(message);
    }

    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        jsonRpc.Dispose();
        OnClose?.Invoke();
        await Task.CompletedTask; // Placeholder for actual close logic
    }

    public event Action OnClose = delegate { };
    public event Action<Exception> OnError = delegate { };
    public event Action<IRequest> OnMessage = delegate { };
}
