using Abstractions.Models;

namespace Client;

public interface IServerResourceConnection
{
    Task<ReadResourceResult> ReadResourceAsync(
        ReadResourceRequest request,
        CancellationToken token = default);
    Task<ListResourcesResult> ListResourcesAsync(
        ListResourcesRequest request,
        CancellationToken token = default);
}