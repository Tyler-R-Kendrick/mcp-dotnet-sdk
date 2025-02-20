using Microsoft.VisualStudio.Threading;
using StreamJsonRpc;
using Abstractions.Models;
using Nerdbank.Streams;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Tests;

[TestClass]
public class TransportTests
{
    [TestMethod, Timeout(2000)]
    public async Task ClientInvokesServerPingMethod()
    {
        //Arrange
        (var clientStream, var serverStream) = FullDuplexStream.CreatePair();
        var services = new ServiceCollection()
            .AddMcpServer(_ => new(serverStream))
            .BuildServiceProvider();
        var server = services.GetRequiredService<IServer>();
        var client = JsonRpc.Attach<IServer>(clientStream);
        PingRequest request = new();

        //Act
        var response = await client
            .PingAsync(request, CancellationToken.None)
            .WithTimeout(TimeSpan.FromSeconds(1));

        //Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType<EmptyResult>(response);
    }
}
