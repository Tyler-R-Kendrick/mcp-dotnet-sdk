using StreamJsonRpc;
using Nerdbank.Streams;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests;
using Abstractions;
using Abstractions.Models;
using Abstractions.Sessions;

[TestClass]
public partial class ClientFactoryTests : McpTestClassFixture<IClientFactory>
{
    [TestMethod, Timeout(2000)]
    public async Task ConnectAsync_ShouldCreateClientConnection()
    {
        (var clientStream, var serverStream) = FullDuplexStream.CreatePair();
        using FakeServerNegotiation serverNegotiation = new(serverStream);
        // Arrange
        Setup(provider => 
        {
            JsonRpc rpc = new(clientStream, serverStream);
            rpc.AddLocalRpcTarget(serverNegotiation);
            rpc.StartListening();
            return rpc;
        });
        Setup(_ => new ClientCapabilities());
        Setup(services => services.AddSingleton<IClientFactory, ClientFactory>());

        // Act
        using var clientConnection = await Concern.ConnectAsync(CancellationToken.None);

        // Assert
        Assert.IsNotNull(clientConnection);
        Assert.AreEqual(1, serverNegotiation.CallCount);
    }
}
