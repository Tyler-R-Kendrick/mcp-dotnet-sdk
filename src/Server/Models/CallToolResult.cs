using Abstractions.Models;

namespace Server.Models;

public record CallToolResult(
    IContent[] Content,
    bool? IsError = false);
