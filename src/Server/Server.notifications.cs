
namespace Server;

using Abstractions.Models;

internal partial class Server
{
    private Task NotifyAsync<TParams>(
        INotification<TParams> notification,
        CancellationToken token = default)
        => notification.NotifyAsync(transport, token);

    public Task SendLoggingMessageAsync(
        LoggingMessageNotification.Parameters @params,
        CancellationToken token = default)
        => NotifyAsync(new LoggingMessageNotification(@params), token);

    public Task SendPromptListChangedAsync(
        PromptListChangedNotification.Parameters @params,
        CancellationToken token = default)
        => NotifyAsync(new PromptListChangedNotification(@params), token);

    public Task SendResourceListChangedAsync(
        ResourceListChangedNotification.Parameters @params,
        CancellationToken token = default)
        => NotifyAsync(new ResourceListChangedNotification(@params), token);

    public Task SendResourceUpdatedAsync(
        ResourceUpdatedNotification.Parameters @params,
        CancellationToken token = default)
        => NotifyAsync(new ResourceUpdatedNotification(@params), token);

    public Task SendToolListChangedAsync(
        ToolListChangedNotification.Parameters @params,
        CancellationToken token = default) 
        => NotifyAsync(new ToolListChangedNotification(@params), token);
}
