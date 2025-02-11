using Abstractions.Models;
using StreamJsonRpc;

namespace Server;

public static class NotificationExtensions
{
    public static Task NotifyAsync<TParams>(
        this INotification<TParams> notification,
        JsonRpc transport, CancellationToken token) => notification == null
            ? throw new ArgumentNullException(nameof(notification))
            : transport.NotifyWithParameterObjectAsync(
                notification.Method, notification.Params);
}
