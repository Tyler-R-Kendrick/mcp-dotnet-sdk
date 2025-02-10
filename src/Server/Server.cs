using Abstractions.Models;
using StreamJsonRpc;

namespace Server;

public interface IServer
{
    ClientCapabilities GetClientCapabilities();

    Implementation GetClientVersion();

#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
    Task Ping(CancellationToken token = default);

    Task<CreateMessageResult> CreateMessage(
        CreateMessageParams @params,
        CancellationToken token = default);

    Task<ListRootsResult> ListRoots(
        ListRootsRequest.Parameters? @params = null,
        CancellationToken token = default);

    Task SendLoggingMessage(
        LoggingMessageNotification.Parameters @params,
        CancellationToken token = default);

    Task SendResourceUpdated(
        ResourceUpdatedNotification.Parameters @params,
        CancellationToken token = default);

    Task SendResourceListChanged(
        ResourceListChangedNotification.Parameters @params,
        CancellationToken token = default);

    Task SendToolListChanged(
        ToolListChangedNotification.Parameters @params,
        CancellationToken token = default);

    Task SendPromptListChanged(
        PromptListChangedNotification.Parameters @params,
        CancellationToken token = default);
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
}
public static class NotificationExtensions
{
    public static Task NotifyAsync<TParams>(
        this INotification<TParams> notification,
        ITransport transport, CancellationToken token) => notification == null
            ? throw new ArgumentNullException(nameof(notification))
            : transport.SendAsync<INotification<TParams>>(
                notification, token);
}

public class Server(
    ITransport transport,
    Implementation implementation,
    ClientCapabilities clientCapabilities,
    ListRootsResult listRootsResult)
    : IServer
{
    public Task<CreateMessageResult> CreateMessage(
        CreateMessageParams @params,
        CancellationToken token = default)
    {
        var cancellationToken = token;
        CreateMessageRequest request = new(@params);
        CreateMessageResult result = new() { };
        return Task.FromResult(result);
    }

    public ClientCapabilities GetClientCapabilities() => clientCapabilities;

    public Implementation GetClientVersion() => implementation;

    public Task<ListRootsResult> ListRoots(
        ListRootsRequest.Parameters? @params = null,
        CancellationToken token = default)
    {
        ListRootsRequest request = new(@params ?? new());
        var listRoots = listRootsResult;
        return Task.FromResult(listRoots);
    }

    public Task Ping(CancellationToken token = default) => Task.CompletedTask;

    private Task NotifyAsync<TParams>(
        INotification<TParams> notification,
        CancellationToken token = default)
        => notification.NotifyAsync(transport, token);

    public Task SendLoggingMessage(
        LoggingMessageNotification.Parameters @params,
        CancellationToken token = default)
        => NotifyAsync(new LoggingMessageNotification(@params), token);

    public Task SendPromptListChanged(
        PromptListChangedNotification.Parameters @params,
        CancellationToken token = default)
        => NotifyAsync(new PromptListChangedNotification(@params), token);

    public Task SendResourceListChanged(
        ResourceListChangedNotification.Parameters @params,
        CancellationToken token = default)
        => NotifyAsync(new ResourceListChangedNotification(@params), token);

    public Task SendResourceUpdated(
        ResourceUpdatedNotification.Parameters @params,
        CancellationToken token = default)
        => NotifyAsync(new ResourceUpdatedNotification(@params), token);

    public Task SendToolListChanged(
        ToolListChangedNotification.Parameters @params,
        CancellationToken token = default) 
        => NotifyAsync(new ToolListChangedNotification(@params), token);
}
