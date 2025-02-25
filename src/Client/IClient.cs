using Abstractions.Models;

namespace Client;

public interface IMcpConnection : IDisposable
{
    // Task<TResult> RequestAsync<TResult, TRequest>(
    //     string targetName,
    //     TRequest? argument = null,
    //     CancellationToken token = default)
    //     where TRequest : class, IRequest;
    // Task NotifyAsync<TRequest>(
    //     string targetName,
    //     TRequest? argument = null,
    //     CancellationToken token = default)
    //     where TRequest : class, IRequest;
}

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
public interface IServerToolConnection
{
    Task<CallToolResult> CallToolAsync(
        CallToolRequest request,
        CancellationToken token = default);
    Task<ListToolsResult> ListToolsAsync(
        ListToolsRequest request,
        CancellationToken token = default);
}
public interface IServerPromptConnection
{
    Task<GetPromptResult> GetPromptAsync(
        GetPromptRequest request,
        CancellationToken token = default);
    Task<ListPromptsResult> ListPromptsAsync(
        ListPromptsRequest request,
        CancellationToken token = default);
}
public interface IServerResourceConnection
{
    Task<ReadResourceResult> ReadResourceAsync(
        ReadResourceRequest request,
        CancellationToken token = default);
    Task<ListResourcesResult> ListResourcesAsync(
        ListResourcesRequest request,
        CancellationToken token = default);
}
public interface IServerUtilityConnection
{
    Task<CompleteResult> CompleteAsync(
        CompleteRequest request,
        CancellationToken token = default);

    Task LogAsync(
        LoggingMessageNotification notification,
        CancellationToken token = default);
}
public interface IServerConnection
    : IServerToolConnection,
    IServerPromptConnection,
    IServerResourceConnection,
    IServerUtilityConnection,
    IMcpConnection
{
}
public interface IClientConnection : IMcpConnection
{
    Task<CreateMessageResult> CreateMessageAsync(
        CreateMessageRequest request,
        CancellationToken token = default);

    Task<ListRootsResult> ListRootsAsync(
        ListRootsRequest request,
        CancellationToken token = default);
}
