
using Microsoft.Extensions.DependencyInjection;
using StreamJsonRpc;

namespace Server;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddJsonRpcStream(
        this IServiceCollection services,
        Stream stream)
        => services.AddSingleton<JsonRpc>(provider => new(stream));
    public static IServiceCollection AddStdioJsonRpc(
        this IServiceCollection services)
        => services.AddSingleton<JsonRpc>(provider => new(
            new HeaderDelimitedMessageHandler(
                Console.OpenStandardInput(),
                Console.OpenStandardOutput())));
    public static IServiceCollection AddMcpServer(
        this IServiceCollection services,
        Func<IServiceProvider, JsonRpc>? jsonRpcFactory = null,
        OnCallToolAsync? callToolHandler = null,
        OnListToolsAsync? listToolsHandler = null,
        OnListPromptsAsync? listPromptsHandler = null,
        OnGetPromptAsync? getPromptHandler = null,
        OnReadResourceAsync? readResourceHandler = null,
        OnListResourcesAsync? listResourcesHandler = null,
        OnCreateMessageAsync? createMessageHandler = null,
        OnCompleteAsync? completeHandler = null)
    {
        return services
            .AddTransient<Func<JsonRpc>>(provider =>
                () => jsonRpcFactory?.Invoke(provider)
                    ?? provider.GetRequiredService<JsonRpc>())
            .AddTransient<JsonRpc>(provider =>
                provider.GetRequiredService<Func<JsonRpc>>()())
            .AddTransient<DelegatedServerFactory>()
            .AddTransient(provider =>
            {
                var rpcServer = provider.GetRequiredService<JsonRpc>();
                var server = provider.GetRequiredService<DelegatedServerFactory>()
                    .Create(
                        callToolHandler: callToolHandler,
                        listToolsHandler: listToolsHandler,
                        getPromptHandler: getPromptHandler,
                        listPromptsHandler: listPromptsHandler,
                        createMessageHandler: createMessageHandler,
                        completeHandler: completeHandler,
                        readResourceHandler: readResourceHandler,
                        listResourcesHandler: listResourcesHandler
                    );
                rpcServer.AddLocalRpcTarget(server);
                rpcServer.StartListening();
                return server;
            })
            .AddTransient<IProtocol>(provider => provider.GetRequiredService<DelegatingServer>())
            .AddTransient<IServer>(provider => provider.GetRequiredService<DelegatingServer>());
    }
}