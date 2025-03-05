namespace Abstractions.Sessions;
using Models;

public partial class ServerSessionProxy(
    IServerSession connection,
    ServerCapabilities serverCapabilities)
    : IServerSession
{
    private bool HasCapability(string messageType)
    {
        return messageType switch
        {
            GetPromptRequest.MethodName =>
                serverCapabilities.Prompts != null,
            ListPromptsRequest.MethodName =>
                serverCapabilities.Prompts != null,
            ReadResourceRequest.MethodName =>
                serverCapabilities.Resources != null,
            ListResourcesRequest.MethodName =>
                serverCapabilities.Resources != null,
            CallToolRequest.MethodName =>
                serverCapabilities.Tools != null,
            ListToolsRequest.MethodName =>
                serverCapabilities.Tools != null,
            _ => false
        };
    }

    private Task<TResult> HandleAsync<TResult>(
        string methodName,
        Func<Task<TResult>> handle)
    {
        return HasCapability(methodName)
            ? handle()
            : throw new InvalidOperationException($"Server does not support {methodName}.");
    }
}

public partial class ServerSessionProxy : IMcpUtility
{
    public Task<ProgressToken> ProgressAsync(
        ProgressNotification notification,
        CancellationToken token = default)
    {
        return connection.ProgressAsync(notification, token);
    }

    public Task<PingResult> PingAsync(
        PingRequest request,
        CancellationToken token = default)
    {
        return connection.PingAsync(request, token);
    }

    public Task CancelAsync(
        CancelledNotification notification,
        CancellationToken token = default)
    {
        return connection.CancelAsync(notification, token);
    }
    
    public virtual void Dispose()
    {
        connection.Dispose();
    }
}

public partial class ServerSessionProxy : IServerUtilitySession
{
    public Task<CompleteResult> CompleteAsync(
        CompleteRequest request,
        CancellationToken token = default)
    {
        return connection.CompleteAsync(request, token);
    }
    
    public Task LogAsync(
        LoggingMessageNotification notification,
        CancellationToken token = default)
    {
        return connection.LogAsync(notification, token);
    }
}

public partial class ServerSessionProxy : IServerPromptSession
{
    public Task<GetPromptResult> GetPromptAsync(
        GetPromptRequest request,
        CancellationToken token = default)
    {
        return HandleAsync(
            GetPromptRequest.MethodName,
            () => connection.GetPromptAsync(request, token));
    }
    
    public Task<ListPromptsResult> ListPromptsAsync(
        ListPromptsRequest request,
        CancellationToken token = default)
    {
        return HandleAsync(
            ListPromptsRequest.MethodName,
            () => connection.ListPromptsAsync(request, token));
    }
}

public partial class ServerSessionProxy : IServerResourceSession
{
    public Task<ReadResourceResult> ReadResourceAsync(
        ReadResourceRequest request,
        CancellationToken token = default)
    {
        return HandleAsync(
            ReadResourceRequest.MethodName,
            () => connection.ReadResourceAsync(request, token));
    }

    public Task<ListResourcesResult> ListResourcesAsync(
        ListResourcesRequest request,
        CancellationToken token = default)
    {
        return HandleAsync(
            ListResourcesRequest.MethodName,
            () => connection.ListResourcesAsync(request, token));
    }
}

public partial class ServerSessionProxy : IServerToolSession
{
    public Task<CallToolResult> CallToolAsync(
        CallToolRequest request,
        CancellationToken token = default)
    {
        return HandleAsync(
            CallToolRequest.MethodName,
            () => connection.CallToolAsync(request, token));
    }

    public Task<ListToolsResult> ListToolsAsync(
        ListToolsRequest request,
        CancellationToken token = default)
    {
        return HandleAsync(
            ListToolsRequest.MethodName,
            () => connection.ListToolsAsync(request, token));
    }
}
