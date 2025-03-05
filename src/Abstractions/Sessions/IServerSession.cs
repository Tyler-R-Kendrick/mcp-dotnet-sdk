namespace Abstractions.Sessions;
using Models;

public interface IServerSession
    : IServerToolSession,
    IServerPromptSession,
    IServerResourceSession,
    IServerUtilitySession,
    IMcpUtility
{
}
