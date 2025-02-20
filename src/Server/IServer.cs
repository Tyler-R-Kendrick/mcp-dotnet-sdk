using Abstractions.Models;

namespace Server;

public interface IServer : IDisposable
{
    Task<CallToolResult> CallToolAsync(CallToolRequest request, CancellationToken token = default);
    Task<ListToolsResult> ListToolsAsync(ListToolsRequest request, CancellationToken token = default);
    Task<GetPromptResult> GetPromptAsync(GetPromptRequest request, CancellationToken token = default);
    Task<ListPromptsResult> ListPromptsAsync(ListPromptsRequest request, CancellationToken token = default);
    Task<CreateMessageResult> CreateMessageAsync(CreateMessageRequest request, CancellationToken token = default);
    Task<CompleteResult> CompleteAsync(CompleteRequest request, CancellationToken token = default);
    Task<ReadResourceResult> ReadResourceAsync(ReadResourceRequest request, CancellationToken token = default);
    Task<ListResourcesResult> ListResourcesAsync(ListResourcesRequest request, CancellationToken token = default);
    Task<EmptyResult> PingAsync(PingRequest request, CancellationToken token = default);
    Task<ListRootsResult> ListRootsAsync(ListRootsRequest request, CancellationToken token = default);
}
