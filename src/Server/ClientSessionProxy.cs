namespace Server;
using Abstractions;
using Abstractions.Models;
using Abstractions.Sessions;

internal partial class ClientSessionProxy(
    IClientSession clientSession,
    ClientCapabilities clientCapabilities)
    : IClientSession
{
    public Task<CreateMessageResult> CreateMessageAsync(
        CreateMessageRequest request,
        CancellationToken token)
    {
        if(clientCapabilities.Sampling == null)
        {
            throw new InvalidOperationException("Client does not support sampling.");
        }
        return clientSession.CreateMessageAsync(request, token)
            ?? throw new InvalidOperationException("Create message handler is not set.");
    }

    public Task<ListRootsResult> ListRootsAsync(
        ListRootsRequest request,
        CancellationToken token = default)
    {
        if(clientCapabilities.Roots == null)
        {
            throw new InvalidOperationException("Client does not support roots.");
        }
        return clientSession.ListRootsAsync(request, token)
            ?? throw new InvalidOperationException("List roots handler is not set.");
    }

    public void Dispose()
    {
        clientSession.Dispose();
    }
}

internal partial class ClientSessionProxy : IMcpUtilities
{
    public Task<ProgressToken> ProgressAsync(
        ProgressNotification notification,
        CancellationToken token = default)
    {
        return clientSession.ProgressAsync(notification, token)
            ?? throw new InvalidOperationException("Progress handler is not set.");
    }
    public Task CancelAsync(
        CancelledNotification notification,
        CancellationToken token = default)
    {
        return clientSession.CancelAsync(notification, token)
            ?? throw new InvalidOperationException("Cancel handler is not set.");
    }
    public Task<PingResult> PingAsync(
        PingRequest request,
        CancellationToken token = default)
    {
        return clientSession.PingAsync(request, token)
            ?? throw new InvalidOperationException("Ping handler is not set.");
    }
}
