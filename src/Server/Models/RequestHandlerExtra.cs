namespace Server.Models;

public record RequestHandlerExtra(
    CancellationToken AbortSignal);
