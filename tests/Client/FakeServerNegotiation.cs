using StreamJsonRpc;

namespace Client.Tests;
using Abstractions;
using Abstractions.Models;

public partial class FakeServerNegotiation(Stream stream)
    : IMcpNegotiation
{
    public int CallCount = 0;

    [JsonRpcMethod("initialize")]
    public async Task<InitializeResult> InitializeAsync(
        InitializeRequest request,
        CancellationToken token = default)
    {
        var responseString = "SUCCESS";
        var repsonse = System.Text.Encoding.UTF8.GetBytes(responseString);
        await stream.WriteAsync(repsonse, token);
        await stream.FlushAsync(token);
        return new() { ProtocolVersion = request.Params.ProtocolVersion };
    }
    public Task NotifyAsync(
        InitializedNotification notification,
        CancellationToken token = default)
    {
        CallCount++;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        stream.Dispose();
    }
}
