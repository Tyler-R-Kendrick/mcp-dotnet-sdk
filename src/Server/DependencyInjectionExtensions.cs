
using Abstractions.Models;
using Microsoft.Extensions.AI;
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
    public static IServiceCollection Configure(
        this IServiceCollection services,
        Action<ServerOptions> configure)
        => services.Configure(configure);
    public static IServiceCollection Configure(
        this IServiceCollection services,
        Action<Implementation> configure)
        => services.Configure(configure);
    public static IServiceCollection Configure(
        this IServiceCollection services,
        Action<ClientCapabilities> configure)
        => services.Configure(configure);
    public static IServiceCollection Configure(
        this IServiceCollection services,
        Action<ListRootsResult> configure)
        => services.Configure(configure);
    public static IServiceCollection AddMcpServer(
        this IServiceCollection services,
        Func<IServiceProvider, JsonRpc>? jsonRpcFactory = null,
        string defaultModel = "gpt-4o",
        Dictionary<string, AIFunction>? tools = null)
    {
        jsonRpcFactory ??= provider => provider.GetRequiredService<JsonRpc>();
        return services
            .AddTransient(jsonRpcFactory)
            .AddSingleton<IProtocol>(provider =>
            {
                var chatClient = provider.GetRequiredService<IChatClient>();
                var rpcServer = provider.GetRequiredService<JsonRpc>();
                T Get<T>() where T : new() => provider.GetService<T>() ?? new();
                var implementation = Get<Implementation>();
                var clientCapabilities = Get<ClientCapabilities>();
                var listRootsResult = Get<ListRootsResult>();
                Server server = new(
                    chatClient: chatClient,
                    transport: rpcServer,
                    implementation: implementation,
                    clientCapabilities: clientCapabilities,
                    listRootsResult: listRootsResult,
                    defaultModel: defaultModel,
                    tools: tools ?? [],
                    pipeline: provider.GetRequiredService<Polly.ResiliencePipeline>(),
                    converter: provider.GetRequiredService<System.ComponentModel.TypeConverter>());
                rpcServer.AddLocalRpcTarget(server);
                rpcServer.StartListening();
                return server;
            });
    }
}