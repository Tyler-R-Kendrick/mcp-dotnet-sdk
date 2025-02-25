using StreamJsonRpc;
using Nerdbank.Streams;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests;
using Abstractions.Models;

[TestClass]
public partial class ClientTests : McpTestClassFixture<ClientFactory>
{
    [TestMethod, Timeout(2000)]
    public async Task ConnectAsync_ShouldCreateClientConnection()
    {
        (var clientStream, var serverStream) = FullDuplexStream.CreatePair();
        using ServerNegotiation serverNegotiation = new(serverStream);
        // Arrange
        Setup(provider => 
        {
            JsonRpc rpc = new(clientStream, serverStream);
            rpc.AddLocalRpcTarget(serverNegotiation);
            rpc.StartListening();
            return rpc;
        });
        Setup(_ => new ClientCapabilities());
        Setup(services => services.AddSingleton<ClientFactory>());

        // Act
        using var clientConnection = await Concern.ConnectAsync(CancellationToken.None);

        // Assert
        Assert.IsNotNull(clientConnection);
        Assert.AreEqual(1, serverNegotiation.CallCount);
    }
}
