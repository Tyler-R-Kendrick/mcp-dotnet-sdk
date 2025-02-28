using Abstractions.Models;

namespace Client;

public interface IServerUtilityConnection
{
    Task<CompleteResult> CompleteAsync(
        CompleteRequest request,
        CancellationToken token = default);

    Task LogAsync(
        LoggingMessageNotification notification,
        CancellationToken token = default);
}