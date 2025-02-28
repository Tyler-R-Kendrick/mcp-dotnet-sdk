namespace Client;
using Abstractions.Models;

public interface IClientConnection : IMcpConnection
{
    Task<CreateMessageResult> CreateMessageAsync(
        CreateMessageRequest request,
        CancellationToken token = default);

    Task<ListRootsResult> ListRootsAsync(
        ListRootsRequest request,
        CancellationToken token = default);
}
