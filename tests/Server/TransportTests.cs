using Microsoft.VisualStudio.Threading;
using StreamJsonRpc;
using Nerdbank.Streams;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Tests;
using Abstractions;
using Abstractions.Models;
using Abstractions.Sessions;

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
        var server = services.GetRequiredService<IServerSession>();
        var client = JsonRpc.Attach<IServerSession>(clientStream);
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
