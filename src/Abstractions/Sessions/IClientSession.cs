namespace Abstractions.Sessions;
using Models;

public interface IClientSession : IMcpUtility
{
    Task<CreateMessageResult> CreateMessageAsync(
        CreateMessageRequest request,
        CancellationToken token = default);

    Task<ListRootsResult> ListRootsAsync(
        ListRootsRequest request,
        CancellationToken token = default);
}
