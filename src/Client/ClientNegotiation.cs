namespace Client;

using Abstractions;
using Abstractions.Models;
using Microsoft.VisualStudio.Threading;
using StreamJsonRpc;

public partial class ClientNegotiation(
    JsonRpc transport,
    ClientCapabilities capabilities)
    : IMcpNegotiation
{
    public async Task<PingResult> PingAsync(
        PingRequest request,
        CancellationToken token = default)
    {
        var response = await transport.InvokeWithParameterObjectAsync<PingResult>(
            targetName: "ping",
            argument: request,
            cancellationToken: token);
        return response;
    }

    public async Task<InitializeResult> InitializeAsync(
        InitializeRequest request,
        CancellationToken token = default)
    {
        var client = transport.Attach<IMcpNegotiation>();
        var response = await client.InitializeAsync(request, token);

        await client.NotifyAsync(
            new InitializedNotification(),
            token);

        return response;
    }

    public async Task NotifyAsync(
        InitializedNotification notification,
        CancellationToken token = default)
    {
        await transport.NotifyWithParameterObjectAsync(
            targetName: "initialized",
            argument: notification)
            .WithCancellation(token);
    }

    public void Dispose()
    {
        transport.Dispose();
    }
}
