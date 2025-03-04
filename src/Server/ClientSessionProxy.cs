namespace Server;
using Abstractions.Sessions;
using Abstractions.Models;

internal class ClientSessionProxy(
    IClientSession clientSession,
    ServerCapabilities serverCapabilities,
    ClientCapabilities clientCapabilities)
    : IClientSession
{
    public Task<CreateMessageResult> CreateMessageAsync(
        Abstractions.Models.CreateMessageRequest request,
        CancellationToken token)
    {
        return clientSession.CreateMessageAsync(request, token)
            ?? throw new InvalidOperationException("Create message handler is not set.");
    }

    public Task<ListRootsResult> ListRootsAsync(
        Abstractions.Models.ListRootsRequest request,
        CancellationToken token = default)
    {
        return clientSession.ListRootsAsync(request, token)
            ?? throw new InvalidOperationException("List roots handler is not set.");
    }

    public void Dispose()
    {
        clientSession.Dispose();
    }
}