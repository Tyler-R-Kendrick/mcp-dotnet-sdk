using Abstractions.Models;

namespace Client;

public interface IServerConnection
    : IServerToolConnection,
    IServerPromptConnection,
    IServerResourceConnection,
    IServerUtilityConnection,
    IMcpConnection
{
}