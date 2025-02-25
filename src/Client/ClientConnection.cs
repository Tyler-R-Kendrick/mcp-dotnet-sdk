
namespace Client;

using Abstractions.Models;

public partial class ClientConnection(
    IServerConnection connection,
    ServerCapabilities serverCapabilities,
    ClientCapabilities clientCapabilities)
    : IClientConnection
{
    public void Dispose()
    {
        connection.Dispose();
    }

    public async Task<CallToolResult> CallToolAsync(
        CallToolRequest request,
        CancellationToken token = default)
    {
        return serverCapabilities.Tools == null
            ? throw new InvalidOperationException("Server does not support tools.")
            : await connection.CallToolAsync(request, token);
    }

    public async Task<ListToolsResult> ListToolsAsync(
        ListToolsRequest request,
        CancellationToken token = default)
    {
        return serverCapabilities.Tools == null
            ? throw new InvalidOperationException("Server does not support tools.")
            : await connection.ListToolsAsync(request, token);
    }

    public async Task<GetPromptResult> GetPromptAsync(
        GetPromptRequest request,
        CancellationToken token = default)
    {
        return serverCapabilities.Prompts == null
            ? throw new InvalidOperationException("Server does not support prompts.")
            : await connection.GetPromptAsync(request, token);
    }
    
    public async Task<ListPromptsResult> ListPromptsAsync(
        ListPromptsRequest request,
        CancellationToken token = default)
    {
        return serverCapabilities.Prompts == null
            ? throw new InvalidOperationException("Server does not support prompts.")
            : await connection.ListPromptsAsync(request, token);
    }

    public Func<CreateMessageRequest, CancellationToken, Task<CreateMessageResult>> OnCreateMessageAsync { get; set; } = default!;
    public async Task<CreateMessageResult> CreateMessageAsync(
        CreateMessageRequest request,
        CancellationToken token = default)
    {
        return clientCapabilities.Sampling == null
            ? throw new InvalidOperationException("Client does not support sampling.")
            : await OnCreateMessageAsync(request, token);
    }

    public async Task<CompleteResult> CompleteAsync(
        CompleteRequest request,
        CancellationToken token = default)
    {
        return await connection.CompleteAsync(request, token);
    }

    public async Task<ReadResourceResult> ReadResourceAsync(
        ReadResourceRequest request,
        CancellationToken token = default)
    {
        return serverCapabilities.Resources == null
            ? throw new InvalidOperationException("Server does not support resources.")
            : await connection.ReadResourceAsync(request, token);
    }
    public async Task<ListResourcesResult> ListResourcesAsync(
        ListResourcesRequest request,
        CancellationToken token = default)
    {
        return serverCapabilities.Resources == null
            ? throw new InvalidOperationException("Server does not support resources.")
            : await connection.ListResourcesAsync(request, token);
    }

    public Func<ListRootsRequest, CancellationToken, Task<ListRootsResult>> OnListRootsAsync { get; set; } = default!;
    public async Task<ListRootsResult> ListRootsAsync(
        ListRootsRequest request,
        CancellationToken token = default)
    {
        return clientCapabilities.Roots == null
            ? throw new InvalidOperationException("Client does not support roots.")
            : await OnListRootsAsync(request, token);
    }
}
