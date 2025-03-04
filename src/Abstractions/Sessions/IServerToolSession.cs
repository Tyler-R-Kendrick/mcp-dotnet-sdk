namespace Abstractions.Sessions;
using Models;

public interface IServerToolSession
{
    Task<CallToolResult> CallToolAsync(
        CallToolRequest request,
        CancellationToken token = default);
    Task<ListToolsResult> ListToolsAsync(
        ListToolsRequest request,
        CancellationToken token = default);
}
