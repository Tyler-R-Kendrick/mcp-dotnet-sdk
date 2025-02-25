using Abstractions.Models;
using StreamJsonRpc;

namespace Server;

public interface IProtocol
{
    Task<IServer> ConnectAsync(CancellationToken token = default);
}
