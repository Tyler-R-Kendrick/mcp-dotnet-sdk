using StreamJsonRpc;

namespace Abstractions;
using Models;

public interface IMcpNegotiation : IDisposable
{
    [JsonRpcMethod("initialize")]
    Task<InitializeResult> InitializeAsync(
        InitializeRequest request,
        CancellationToken token = default);

    [JsonRpcMethod("initialized")]
    Task NotifyAsync(
        InitializedNotification notification,
        CancellationToken token = default);
}
