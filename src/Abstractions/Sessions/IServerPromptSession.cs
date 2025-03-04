namespace Abstractions.Sessions;
using Models;

public interface IServerPromptSession
{
    Task<GetPromptResult> GetPromptAsync(
        GetPromptRequest request,
        CancellationToken token = default);
    Task<ListPromptsResult> ListPromptsAsync(
        ListPromptsRequest request,
        CancellationToken token = default);
}
