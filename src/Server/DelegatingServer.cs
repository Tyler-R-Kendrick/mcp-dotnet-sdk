using Abstractions.Models;
using StreamJsonRpc;

namespace Server;
using Models;
using Abstractions;
using Abstractions.Models;
using Abstractions.Sessions;

internal partial class DelegatingServer(
    JsonRpc transport)
    : IServerSession, IDisposable
{
    public void Dispose() => transport.Dispose();
}

internal partial class DelegatingServer : IServerToolSession
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

internal partial class DelegatingServer : IServerPromptSession
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

internal partial class DelegatingServer : IServerUtilitySession
{
    public OnLogAsync? LogHandler;
    public Task LogAsync(
        Abstractions.Models.LoggingMessageNotification request,
        CancellationToken token = default)
    {
        return LogHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("Log handler is not set.");
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

internal partial class DelegatingServer : IServerResourceSession
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

internal partial class DelegatingServer : IMcpNegotiation
{
    public OnInitializeAsync? InitializeHandler;
    public Task<InitializeResult> InitializeAsync(
        Abstractions.Models.InitializeRequest request,
        CancellationToken token = default)
    {
        return InitializeHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("Initialize handler is not set.");
    }

    public OnInitializedAsync? InitializedHandler;
    public Task NotifyAsync(
        Abstractions.Models.InitializedNotification request,
        CancellationToken token = default)
    {
        return InitializedHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("Initialized handler is not set.");
    }
    
    public OnPingAsync? PingHandler = delegate { return Task.FromResult(new PingResult()); };
    public Task<PingResult> PingAsync(
        Abstractions.Models.PingRequest request,
        CancellationToken token = default)
    {
        return PingHandler?.Invoke(request, token)
            ?? throw new InvalidOperationException("Ping handler is not set.");
    }
}
