using Abstractions.Models;

namespace Client;

public interface IServerPromptConnection
{
    Task<GetPromptResult> GetPromptAsync(
        GetPromptRequest request,
        CancellationToken token = default);
    Task<ListPromptsResult> ListPromptsAsync(
        ListPromptsRequest request,
        CancellationToken token = default);
}