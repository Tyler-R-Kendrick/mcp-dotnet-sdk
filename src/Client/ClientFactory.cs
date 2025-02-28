using StreamJsonRpc;

namespace Client;
using Abstractions.Models;

public interface IClientFactory
{
    Task<PingResult> PingAsync(
        PingRequest request,
        CancellationToken token = default);

    Task<IClientConnection> ConnectAsync(
        CancellationToken token = default);
}
internal class ClientFactory(
    JsonRpc transport,
    ClientCapabilities capabilities,
    OnCreateMessageAsync? onCreateMessageAsync = null,
    OnListRootsAsync? onListRootsAsync = null)
    : IClientFactory
{
    public async Task<PingResult> PingAsync(
        PingRequest request,
        CancellationToken token = default)
    {
        ClientNegotiation client = new(transport, capabilities);
        var result = await client.PingAsync(request, token);
        client.Dispose();
        return result;
    }

    public async Task<IClientConnection> ConnectAsync(
        CancellationToken token = default)
    {
        ClientNegotiation client = new(transport, capabilities);
        InitializeRequest request = new(new());
        var result = await client.InitializeAsync(
            request,
            token);
        if(request.Params.ProtocolVersion != result.ProtocolVersion)
        {
            client.Dispose();
            throw new InvalidOperationException("Protocol version mismatch.");
        }
        await client.NotifyAsync(
            new(),
            token);

        IServerConnection serverConnection = transport.Attach<IServerConnection>();
        return new ClientConnection(
            connection: serverConnection,
            serverCapabilities: result.Capabilities,
            clientCapabilities: capabilities)
        {
            OnCreateMessageAsync = onCreateMessageAsync,
            OnListRootsAsync = onListRootsAsync,
        };
    }
}