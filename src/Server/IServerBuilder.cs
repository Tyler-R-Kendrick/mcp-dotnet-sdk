using Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Server;

public interface IServerBuilder
{
    IServerBuilder AddTool(Delegate implementation);
    IServerBuilder AddResource(Resource resource);
    IServerBuilder AddPrompt(Prompt prompt);
    IProtocol Build();
}

public static class ServerBuilderExtensions
{
    public static IServerBuilder AddResource(
        this IServerBuilder builder,
        string uri)
        => !Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute)
            ? throw new ArgumentException("The URI is not well formed.", nameof(uri))
            : builder.AddResource(new Uri(uri));

    public static IServerBuilder AddResource(
        this IServerBuilder builder,
        Uri uri)
        => builder.AddResource(new() { Uri = uri });
}

internal record ServerBuilder(
    IServiceCollection Services)
    : IServerBuilder
{
    private HashSet<Tool> _tools = [];
    public IServerBuilder AddTool(Delegate implementation)
    {
        var tool = implementation.ToTool();
        return this with { _tools = [.. _tools, tool] };
    }

    private HashSet<Resource> _resources = [];
    public IServerBuilder AddResource(Resource resource)
        => this with { _resources = [.. _resources, resource] };

    private HashSet<Prompt> _prompts = [];
    public IServerBuilder AddPrompt(Prompt prompt)
        => this with { _prompts = [.. _prompts, prompt] };

    public IProtocol Build()
    {
        var server = Services
            .BuildServiceProvider()
            .GetRequiredService<Server>();
        // foreach (var tool in _tools)
        //     server.Tools.Add(tool);
        // foreach (var resource in _resources)
        //     server.Resources.Add(resource);
        // foreach (var prompt in _prompts)
        //     server.Prompts.Add(prompt);
        return server;
    }
}
