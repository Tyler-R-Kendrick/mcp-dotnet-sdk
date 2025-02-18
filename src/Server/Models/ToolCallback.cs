namespace Server.Models;

public delegate Task<CallToolResult> ToolCallback<TArgs>(
    TArgs args,
    CancellationToken token);

public delegate Task<CallToolResult> ToolCallback(
    CancellationToken token);
