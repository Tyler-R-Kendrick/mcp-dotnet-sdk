using Abstractions.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Tests;

[TestClass]
public class ToolCallTests
{
    private static IServiceCollection Configure(
        Dictionary<string, AIFunction>? tools = null
    )
    {
        ServiceCollection services = new();
        services.AddMcpServer(tools: tools);
        return services;
    }

    [TestMethod]
    public async Task CallToolAsync_ReturnsExpectedResult()
    {
        // Arrange
        var tool = AIFunctionFactory.Create(() => "value", new() { });
        var tools = new Dictionary<string, AIFunction>
        {
            { "TestTool", tool }
        };
        var services = Configure(tools);
        var serviceProvider = services.BuildServiceProvider();
        var server = serviceProvider.GetRequiredService<Server>();
        CallToolRequest request = new(new()
        {
            Name = tool.Metadata.Name
        });

        // Act
        var result = await server.CallToolAsync(request, default);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType<IContent[]>(result.Content);
    }

    [TestMethod]
    public async Task CallToolAsync_ReturnsErrorOnException()
    {
        // Arrange
        var services = Configure();
        var serviceProvider = services.BuildServiceProvider();
        var server = serviceProvider.GetRequiredService<Server>();
        CallToolRequest request = new(new()
        {
            Name = "NonExistentTool",
            Arguments = new() { { "arg1", "value1" } }
        });

        // Act
        var result = await server.CallToolAsync(request, default);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsError);
    }
}
