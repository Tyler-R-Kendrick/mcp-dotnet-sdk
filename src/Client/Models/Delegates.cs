namespace Client.Models;
using Abstractions.Models;

public delegate Task<CreateMessageResult> OnCreateMessageAsync(
    CreateMessageRequest request,
    CancellationToken token);

public delegate Task<ListRootsResult> OnListRootsAsync(
    ListRootsRequest request,
    CancellationToken token);
