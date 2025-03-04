using StreamJsonRpc;
using Nerdbank.Streams;

namespace Client.Tests;
using Abstractions.Models;
using Abstractions.Sessions;

[TestClass]
public partial class ClientConnectionTests : McpTestClassFixture<IClientSession>
{
    [TestMethod, Timeout(2000)]
    public async Task CreateMessageAsync_ShouldSucceed()
    {
        // Arrange
        (var clientStream, var serverStream) = FullDuplexStream.CreatePair();
        using FakeServerNegotiation serverNegotiation = new(serverStream);
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

        // Act
        using var connection = Concern;
        var response = await connection.CreateMessageAsync(new(new()), default);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(1, serverNegotiation.CallCount);
    }

    [TestMethod, Timeout(2000)]
    public async Task ListRootsAsync_ShouldSucceed()
    {
        // Arrange
        (var clientStream, var serverStream) = FullDuplexStream.CreatePair();
        using FakeServerNegotiation serverNegotiation = new(serverStream);
        Setup(services => services.AddMcpClient(
            jsonRpcFactory: (provider, _) => 
            {
                JsonRpc rpc = new(clientStream, serverStream);
                rpc.AddLocalRpcTarget(serverNegotiation);
                rpc.StartListening();
                return rpc;
            },
            clientCapabilitiesFactory: (_, _) => new() { Roots = new() },
            onListRootsAsync: (_, _) => Task.FromResult(new ListRootsResult())
        ));

        // Act
        using var connection = Concern;
        var response = await connection.ListRootsAsync(new(new()), default);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(1, serverNegotiation.CallCount);
    }
}
