namespace Abstractions;
using Models;
using Sessions;

public interface IClientFactory
{
    Task<PingResult> PingAsync(
        PingRequest request,
        CancellationToken token = default);

    Task<IClientSession> ConnectAsync(
        CancellationToken token = default);
}
