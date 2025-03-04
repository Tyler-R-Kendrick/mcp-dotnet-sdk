using Microsoft.Extensions.DependencyInjection;
using StreamJsonRpc;
using Microsoft.VisualStudio.Threading;

namespace Client;
using Models;
using Abstractions;
using Abstractions.Models;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMcpClient(
        this IServiceCollection services,
        Func<IServiceProvider, object?, JsonRpc> jsonRpcFactory,
        Func<IServiceProvider, object?, ClientCapabilities>? clientCapabilitiesFactory = null,
        OnCreateMessageAsync? onCreateMessageAsync = null,
        OnListRootsAsync? onListRootsAsync = null,
        string? key = null)
        {
            clientCapabilitiesFactory ??= (provider, _) => new() { Sampling = onCreateMessageAsync is null ? null : [] };
            return services
                .AddKeyedSingleton<JsonRpc>(key, jsonRpcFactory)
                .AddKeyedSingleton<ClientCapabilities>(key, clientCapabilitiesFactory)            
                .AddKeyedSingleton<IClientFactory, ClientFactory>(key,
                    (provider, k) => new(
                        transport: provider.GetRequiredKeyedService<JsonRpc>(k), 
                        capabilities: provider.GetRequiredKeyedService<ClientCapabilities>(k),
                        onCreateMessageAsync: onCreateMessageAsync,
                        onListRootsAsync: onListRootsAsync))
                .AddKeyedSingleton(key, (provider, t) => 
                {
                    var clientFactory = provider.GetRequiredKeyedService<IClientFactory>(key);
                    var jsonRpc = provider.GetRequiredKeyedService<JsonRpc>(key);
                    var taskFactory = jsonRpc.JoinableTaskFactory ?? new(new JoinableTaskContext());
                    return taskFactory.Run(async () => await clientFactory.ConnectAsync());
                });
                
        }
}
