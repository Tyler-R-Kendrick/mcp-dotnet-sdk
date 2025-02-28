using Abstractions.Models;

namespace Client;

public interface IMcpConnection : IDisposable
{
    // Task<TResult> RequestAsync<TResult, TRequest>(
    //     string targetName,
    //     TRequest? argument = null,
    //     CancellationToken token = default)
    //     where TRequest : class, IRequest;
    // Task NotifyAsync<TRequest>(
    //     string targetName,
    //     TRequest? argument = null,
    //     CancellationToken token = default)
    //     where TRequest : class, IRequest;
}