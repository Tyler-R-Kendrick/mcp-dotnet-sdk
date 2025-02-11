using Abstractions.Models;
using StreamJsonRpc;

namespace Server;

public interface IProtocol
{
    [JsonRpcMethod("registerCapabilites")]
    Task RegisterCapabilitesAsync(
        ServerCapabilities capabilities,
        CancellationToken cancellationToken = default);

    [JsonRpcMethod("getClientCapabilities")]
    Task<ClientCapabilities> GetClientCapabilitiesAsync(
        CancellationToken cancellationToken = default);

    [JsonRpcMethod("getClientVersion")]
    Task<Implementation> GetClientVersionAsync(
        CancellationToken cancellationToken = default);

    [JsonRpcMethod("ping")]
    Task<EmptyResult> PingAsync(    
        CancellationToken cancellationToken = default);

    [JsonRpcMethod("createMessage")]
    Task<CreateMessageResult> CreateMessageAsync(
        CreateMessageRequest.Parameters request,
        CancellationToken cancellationToken = default);

    [JsonRpcMethod("listRoots")]
    Task<ListRootsResult> ListRootsAsync(
        ListRootsRequest.Parameters request,
        CancellationToken cancellationToken = default);

    [JsonRpcMethod("sendLoggingMessage")]
    Task SendLoggingMessageAsync(
        LoggingMessageNotification.Parameters request,
        CancellationToken cancellationToken = default);

    [JsonRpcMethod("sendResourceUpdated")]
    Task SendResourceUpdatedAsync(
        ResourceUpdatedNotification.Parameters request,
        CancellationToken cancellationToken = default);

    [JsonRpcMethod("sendResourceListChanged")]
    Task SendResourceListChangedAsync(
        ResourceListChangedNotification.Parameters request,
        CancellationToken cancellationToken = default);

    [JsonRpcMethod("sendToolListChanged")]
    Task SendToolListChangedAsync(
        ToolListChangedNotification.Parameters request,
        CancellationToken cancellationToken = default);

    [JsonRpcMethod("sendPromptListChanged")]
    Task SendPromptListChangedAsync(
        PromptListChangedNotification.Parameters request,
        CancellationToken cancellationToken = default);
}
