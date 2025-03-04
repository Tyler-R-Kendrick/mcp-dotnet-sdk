using StreamJsonRpc;

namespace Client;
using Models;
using Abstractions;
using Abstractions.Models;
using Abstractions.Sessions;

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
        using ClientNegotiation client = new(transport, capabilities);
        return await client.PingAsync(request, token);
    }

    public async Task<IClientSession> ConnectAsync(
        CancellationToken token = default)
    {
        using ClientNegotiation client = new(transport, capabilities);
        InitializeRequest request = new(new());
        var result = await client.InitializeAsync(
            request,
            token);
        if(request.Params.ProtocolVersion != result.ProtocolVersion)
        {
            throw new InvalidOperationException("Protocol version mismatch.");
        }
        await client.NotifyAsync(new(), token);

        IServerSession serverConnection = transport.Attach<IServerSession>();
        return new DelegateClientSession(
            serverSession: serverConnection,
            serverCapabilities: result.Capabilities,
            clientCapabilities: capabilities)
        {
            OnCreateMessageAsync = onCreateMessageAsync,
            OnListRootsAsync = onListRootsAsync,
        };
    }
}
