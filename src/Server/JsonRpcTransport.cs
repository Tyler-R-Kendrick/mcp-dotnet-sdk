using Abstractions.Models;
using StreamJsonRpc;

namespace Server;

public class ServerJsonRpcTransport(JsonRpc jsonRpc) : ITransport
{
    public async Task<TResult> SendAsync<TResult>(
        IMessage message,
        CancellationToken cancellationToken)
    {
        var response = await jsonRpc.InvokeWithCancellationAsync<TResult>(
            message.Method,
            [message],
            cancellationToken);
        return response;
    }

    public IDisposable Subscribe(IObserver<IMessage> observer)
    {
        throw new NotImplementedException();
    }
}
