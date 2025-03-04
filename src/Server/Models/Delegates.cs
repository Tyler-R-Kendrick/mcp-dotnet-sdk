namespace Server.Models;
using Abstractions.Models;

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

public delegate Task<PingResult> OnPingAsync(
    Abstractions.Models.PingRequest request,
    CancellationToken token);

public delegate Task<ListRootsResult> OnListRootsAsync(
    Abstractions.Models.ListRootsRequest request,
    CancellationToken token = default);

public delegate Task<InitializeResult> OnInitializeAsync(
    Abstractions.Models.InitializeRequest request,
    CancellationToken token = default);

public delegate Task OnInitializedAsync(
    Abstractions.Models.InitializedNotification request,
    CancellationToken token = default);

public delegate Task OnLogAsync(
    Abstractions.Models.LoggingMessageNotification request,
    CancellationToken token = default);
