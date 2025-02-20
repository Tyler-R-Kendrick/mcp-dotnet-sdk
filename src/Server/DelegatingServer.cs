using Abstractions.Models;
using StreamJsonRpc;

namespace Server;

public delegate Task<Abstractions.Models.CreateMessageResult> OnCreateMessageAsync(
    Abstractions.Models.CreateMessageRequest request,
    CancellationToken token);
    
public delegate Task<Abstractions.Models.ListToolsResult> OnListToolsAsync(
    Abstractions.Models.ListToolsRequest request,
    CancellationToken token);

public delegate Task<Abstractions.Models.CallToolResult> OnCallToolAsync(
    Abstractions.Models.CallToolRequest request,
    CancellationToken token);

public delegate Task<Abstractions.Models.ListPromptsResult> OnListPromptsAsync(
    Abstractions.Models.ListPromptsRequest request,
    CancellationToken token);

public delegate Task<Abstractions.Models.GetPromptResult> OnGetPromptAsync(
    Abstractions.Models.GetPromptRequest request,
    CancellationToken token);
    
public delegate Task<Abstractions.Models.ListResourcesResult> OnListResourcesAsync(
    Abstractions.Models.ListResourcesRequest request,
    CancellationToken token);

public delegate Task<Abstractions.Models.ReadResourceResult> OnReadResourceAsync(
    Abstractions.Models.ReadResourceRequest request,
    CancellationToken token);

public delegate Task<Abstractions.Models.CompleteResult> OnCompleteAsync(
    Abstractions.Models.CompleteRequest request,
    CancellationToken token);

internal partial class DelegatingServer(
    JsonRpc transport)
    : IProtocol, IDisposable
{
    public void Dispose() => transport.Dispose();
}

internal partial class DelegatingServer : IServer
{
    public OnCallToolAsync? CallToolHandler;
    public Task<CallToolResult> CallToolAsync(
        Abstractions.Models.CallToolRequest request,
        CancellationToken token = default)
    {
        return CallToolHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("Call tool handler is not set.");
    }

    public OnListToolsAsync? ListToolsHandler;
    public Task<ListToolsResult> ListToolsAsync(
        Abstractions.Models.ListToolsRequest request,
        CancellationToken token = default)
    {
        return ListToolsHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("List tools handler is not set.");
    }
}

internal partial class DelegatingServer
{
    public OnGetPromptAsync? GetPromptHandler;
    public Task<GetPromptResult> GetPromptAsync(
        Abstractions.Models.GetPromptRequest request,
        CancellationToken token = default)
    {
        return GetPromptHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("Get prompt handler is not set.");
    }

    public OnListPromptsAsync? ListPromptsHandler;
    public Task<ListPromptsResult> ListPromptsAsync(
        Abstractions.Models.ListPromptsRequest request,
        CancellationToken token = default)
    {
        return ListPromptsHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("List prompts handler is not set.");
    }
}

internal partial class DelegatingServer
{
    public OnCreateMessageAsync? CreateMessageHandler;
    public Task<CreateMessageResult> CreateMessageAsync(
        Abstractions.Models.CreateMessageRequest request,
        CancellationToken token)
    {
        return CreateMessageHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("Create message handler is not set.");
    }
    
    public OnCompleteAsync? CompleteHandler;
    public Task<CompleteResult> CompleteAsync(
        Abstractions.Models.CompleteRequest request,
        CancellationToken token = default)
    {
        return CompleteHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("Complete handler is not set.");
    }
}

internal partial class DelegatingServer
{
    public OnReadResourceAsync? ReadResourceHandler;
    public Task<ReadResourceResult> ReadResourceAsync(
        Abstractions.Models.ReadResourceRequest request,
        CancellationToken token = default)
    {
        return ReadResourceHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("Read resource handler is not set.");
    }

    public OnListResourcesAsync? ListResourcesHandler;
    public Task<ListResourcesResult> ListResourcesAsync(
        Abstractions.Models.ListResourcesRequest request,
        CancellationToken token = default)
    {
        return ListResourcesHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("List resources handler is not set.");
    }
}

public delegate Task<EmptyResult> OnPingAsync(
    Abstractions.Models.PingRequest request,
    CancellationToken token);
internal partial class DelegatingServer
{
    public OnPingAsync? PingHandler = delegate { return Task.FromResult(new EmptyResult()); };
    public Task<EmptyResult> PingAsync(
        Abstractions.Models.PingRequest request,
        CancellationToken token = default)
    {
        return PingHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("Ping handler is not set.");
    }
}

public delegate Task<ListRootsResult> OnListRootsAsync(
        Abstractions.Models.ListRootsRequest request,
        CancellationToken token = default);
internal partial class DelegatingServer
{
    public OnListRootsAsync? ListRootsHandler;
    public Task<ListRootsResult> ListRootsAsync(
        Abstractions.Models.ListRootsRequest request,
        CancellationToken token = default)
    {
        return ListRootsHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("List roots handler is not set.");
    }
}

internal class DelegatedServerFactory(Func<JsonRpc> transportFactory)
{
    public DelegatingServer Create(
        OnCallToolAsync? callToolHandler = null,
        OnListToolsAsync? listToolsHandler = null,
        OnGetPromptAsync? getPromptHandler = null,
        OnListPromptsAsync? listPromptsHandler = null,
        OnCreateMessageAsync? createMessageHandler = null,
        OnCompleteAsync? completeHandler = null,
        OnReadResourceAsync? readResourceHandler = null,
        OnListResourcesAsync? listResourcesHandler = null)
    => new(transportFactory())
    {
        CallToolHandler = callToolHandler,
        ListToolsHandler = listToolsHandler,
        GetPromptHandler = getPromptHandler,
        ListPromptsHandler = listPromptsHandler,
        CreateMessageHandler = createMessageHandler,
        CompleteHandler = completeHandler,
        ReadResourceHandler = readResourceHandler,
        ListResourcesHandler = listResourcesHandler
    };
}
