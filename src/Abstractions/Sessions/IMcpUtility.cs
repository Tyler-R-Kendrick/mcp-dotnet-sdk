namespace Abstractions.Sessions;
using Models;

public interface IMcpUtility : IDisposable
{
    Task<PingResult> PingAsync(
        PingRequest request,
        CancellationToken token = default);

    Task CancelAsync(
        CancelledNotification notification,
        CancellationToken token = default);

    Task<ProgressToken> ProgressAsync(
        ProgressNotification notification,
        CancellationToken token = default);
}