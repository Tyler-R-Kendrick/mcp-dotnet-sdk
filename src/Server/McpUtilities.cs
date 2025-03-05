namespace Server;
using Abstractions;
using Abstractions.Models;

internal class McpUtilities : IMcpUtilities
{
    public Task<PingResult> PingAsync(
        PingRequest request,
        CancellationToken token = default)
    {
        return Task.FromResult(new PingResult());
    }

    public Task CancelAsync(
        CancelledNotification notification,
        CancellationToken token = default)
    {
        //TODO: Implement cancellation logic
        throw new NotImplementedException("Cancellation is not implemented.");
    }

    public Task<ProgressToken> ProgressAsync(
        ProgressNotification notification,
        CancellationToken token = default)
    {
        throw new NotImplementedException("Progress is not implemented.");
    }

    public void Dispose()
    {
    }
}
