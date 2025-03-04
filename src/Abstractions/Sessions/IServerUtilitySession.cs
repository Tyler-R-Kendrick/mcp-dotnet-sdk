namespace Abstractions.Sessions;
using Models;

public interface IServerUtilitySession
{
    Task<PingResult> PingAsync(
        PingRequest request,
        CancellationToken token = default);

    Task<CompleteResult> CompleteAsync(
        CompleteRequest request,
        CancellationToken token = default);

    Task LogAsync(
        LoggingMessageNotification notification,
        CancellationToken token = default);
}
