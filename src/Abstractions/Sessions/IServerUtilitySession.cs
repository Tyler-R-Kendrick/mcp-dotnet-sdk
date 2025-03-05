namespace Abstractions.Sessions;
using Models;

public interface IServerUtilitySession
{
    Task<CompleteResult> CompleteAsync(
        CompleteRequest request,
        CancellationToken token = default);

    Task LogAsync(
        LoggingMessageNotification notification,
        CancellationToken token = default);
}
