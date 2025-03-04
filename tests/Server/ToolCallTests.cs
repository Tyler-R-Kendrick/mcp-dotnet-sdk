using Microsoft.Extensions.DependencyInjection;

namespace Server.Tests;
using Models;
using Abstractions.Models;
using Abstractions.Sessions;

[TestClass]
public class ToolCallTests
{
    private static IServerSession Allocate(
        OnCallToolAsync? callToolHandler = null)
    {
        var services = Configure(callToolHandler: callToolHandler);
        services.AddJsonRpcStream(new MemoryStream());
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IServerSession>();
    }

    private static IServiceCollection Configure(
        OnCallToolAsync? callToolHandler = null)
    {
        ServiceCollection services = new();
        services.AddMcpServer(
            callToolHandler: callToolHandler);
        return services;
    }

    [TestMethod, Timeout(2000)]
    public async Task CallToolAsync_ReturnsExpectedResult()
    {
        // Arrange
        int toolCallCount = 0;
        var server = Allocate(callToolHandler: (request, token) =>
        {
            toolCallCount++;
            return Task.FromResult(new CallToolResult());
        });
        CallToolRequest request = new(new()
        {
            Name = "TestTool",
            Arguments = new() { { "arg1", "value1" } }
        });

        // Act
        var result = await server.CallToolAsync(request, default);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType<IContent[]>(result.Content);
        Assert.AreEqual(1, toolCallCount);
    }

    [TestMethod, Timeout(2000)]
    public async Task CallToolAsync_ReturnsErrorOnException()
    {
        var server = Allocate(callToolHandler: (request, token) =>
        {
            CallToolResult result = new() { IsError = true };
            return Task.FromResult(result);
        });
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
