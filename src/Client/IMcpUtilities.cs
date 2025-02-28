using Abstractions.Models;

namespace Client;

public interface IMcpUtilities : IDisposable
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