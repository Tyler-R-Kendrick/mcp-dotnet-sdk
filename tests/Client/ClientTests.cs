using StreamJsonRpc;
using Nerdbank.Streams;

namespace Client.Tests;
using Abstractions.Models;

[TestClass]
public partial class ClientConnectionTests : McpTestClassFixture<IClientFactory>
{
    [TestMethod, Timeout(2000)]
    public async Task ConnectAsync_ShouldCreateMessage()
    {
        (var clientStream, var serverStream) = FullDuplexStream.CreatePair();
        using FakeServerNegotiation serverNegotiation = new(serverStream);
        // Arrange
        Setup(services => services.AddMcpClient(
            jsonRpcFactory: (provider, _) => 
            {
                JsonRpc rpc = new(clientStream, serverStream);
                rpc.AddLocalRpcTarget(serverNegotiation);
                rpc.StartListening();
                return rpc;
            },
            clientCapabilitiesFactory: (_, _) => new() { Sampling = [] },
            onCreateMessageAsync: (_, _) => Task.FromResult(new CreateMessageResult())
        ));
        CreateMessageRequest request = new(new());

        // Act
        using IClientConnection connection = await Concern.ConnectAsync();
        var response = await connection.CreateMessageAsync(request, CancellationToken.None);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(1, serverNegotiation.CallCount);
    }
}
