using Abstractions.Models;

namespace Server.Models;

public record RegisteredTool(
    ToolCallback ToolCallback)
    : Tool();

public record RegisteredTool<TArgs>(
    ToolCallback<TArgs> ToolCallback)
    : Tool();
