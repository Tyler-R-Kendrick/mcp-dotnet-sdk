using Microsoft.VisualStudio.Threading;
using StreamJsonRpc;
using Server;
using Abstractions.Models;
using Nerdbank.Streams;
using Microsoft.Extensions.DependencyInjection;

namespace Generators.Tests;

[TestClass]
public class TransportTests
{
    [TestMethod]
    public async Task ClientInvokesServerPingMethod()
    {
        //Arrange
        (var clientStream, var serverStream) = FullDuplexStream.CreatePair();
        var services = new ServiceCollection()
            .AddMcpServer(_ => new(serverStream))
            .BuildServiceProvider();
        var server = services.GetRequiredService<IProtocol>();
        var client = JsonRpc.Attach<IProtocol>(clientStream);

        //Act
        var response = await client
            .PingAsync(CancellationToken.None)
            .WithTimeout(TimeSpan.FromSeconds(1));

        //Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType<EmptyResult>(response);
    }
}
