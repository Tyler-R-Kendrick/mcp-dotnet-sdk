
using Abstractions.Models;

namespace Server;

public record ServerOptions(
    ServerCapabilities? Capabilities,
    string? Instructions,
    bool? EnforceStrictCapabilities)
    : ProtocolOptions(EnforceStrictCapabilities);
