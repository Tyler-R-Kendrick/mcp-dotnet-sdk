namespace Client;

using Models;
using Abstractions.Models;
using Abstractions.Sessions;

public partial class DelegateClientSession(
    IServerSession serverSession,
    ServerCapabilities serverCapabilities,
    ClientCapabilities clientCapabilities)
    : ServerSessionProxy(serverSession, serverCapabilities), IMcpSession 
{
    public OnCreateMessageAsync? OnCreateMessageAsync { get; set; } = default!;
    public async Task<CreateMessageResult> CreateMessageAsync(
        CreateMessageRequest request,
        CancellationToken token = default)
    {
        return clientCapabilities.Sampling == null || OnCreateMessageAsync == null
            ? throw new InvalidOperationException("Client does not support sampling.")
            : await OnCreateMessageAsync(request, token);
    }

    public OnListRootsAsync? OnListRootsAsync { get; set; } = default!;
    public async Task<ListRootsResult> ListRootsAsync(
        ListRootsRequest request,
        CancellationToken token = default)
    {
        return clientCapabilities.Roots == null || OnListRootsAsync == null
            ? throw new InvalidOperationException("Client does not support roots.")
            : await OnListRootsAsync(request, token);
    }
}
