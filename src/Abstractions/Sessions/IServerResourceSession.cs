namespace Abstractions.Sessions;
using Models;

public interface IServerResourceSession
{
    Task<ReadResourceResult> ReadResourceAsync(
        ReadResourceRequest request,
        CancellationToken token = default);
    Task<ListResourcesResult> ListResourcesAsync(
        ListResourcesRequest request,
        CancellationToken token = default);
}
