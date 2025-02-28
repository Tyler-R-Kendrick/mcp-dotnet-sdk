using Abstractions.Models;

namespace Client;

public interface IServerToolConnection
{
    Task<CallToolResult> CallToolAsync(
        CallToolRequest request,
        CancellationToken token = default);
    Task<ListToolsResult> ListToolsAsync(
        ListToolsRequest request,
        CancellationToken token = default);
}