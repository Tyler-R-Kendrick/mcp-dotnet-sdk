using Microsoft.VisualStudio.Threading;
using StreamJsonRpc;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Server;
using Abstractions.Models;
using Nerdbank.Streams;
using Microsoft.Extensions.DependencyInjection;

namespace Generators.Tests;

[TestClass]
public class MinimalTests
{
    [TestMethod]
    public async Task JsonRpcInvokesServerMethod()
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
