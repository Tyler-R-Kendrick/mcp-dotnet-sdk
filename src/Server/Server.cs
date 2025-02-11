using Abstractions.Models;
using StreamJsonRpc;

namespace Server;

internal class Server(
    JsonRpc transport,
    Implementation implementation,
    ClientCapabilities clientCapabilities,
    ListRootsResult listRootsResult)
    : IProtocol, IDisposable
{
    public Task<CreateMessageResult> CreateMessageAsync(
        CreateMessageRequest.Parameters @params,
        CancellationToken token = default)
    {
        var cancellationToken = token;
        CreateMessageRequest request = new(@params);
        CreateMessageResult result = new()
        {
        };
        return Task.FromResult(result);
    }

    public Task<ClientCapabilities> GetClientCapabilitiesAsync(
        CancellationToken token = default) => Task.FromResult(clientCapabilities);

    public Task<Implementation> GetClientVersionAsync(
        CancellationToken token = default) => Task.FromResult(implementation);

    public Task<ListRootsResult> ListRootsAsync(
        ListRootsRequest.Parameters? @params = null,
        CancellationToken token = default)
    {
        ListRootsRequest request = new(@params ?? new());
        var listRoots = listRootsResult;
        return Task.FromResult(listRoots);
    }

    [JsonRpcMethod("ping")]
    public Task<EmptyResult> PingAsync(CancellationToken token = default)
        => Task.FromResult(new EmptyResult());

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

    private ServerCapabilities _capabilities = new();
    public Task RegisterCapabilitesAsync(
        ServerCapabilities capabilities,
        CancellationToken cancellationToken = default)
    {
        _capabilities = capabilities;
        return Task.CompletedTask;
    }

    public void Dispose() => transport.Dispose();
}
