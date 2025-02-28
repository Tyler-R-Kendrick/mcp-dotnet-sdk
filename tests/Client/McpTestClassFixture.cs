using StreamJsonRpc;
using Nerdbank.Streams;

namespace Client.Tests;

public partial class McpTestClassFixture<TConcern>
    : TestClassFixture<TConcern>
    where TConcern : class
{
    protected virtual void SetupJsonRpc(JsonRpc rpc, object rpcTarget)
    {
        Setup(provider => 
        {
            rpc.AddLocalRpcTarget(rpcTarget);
            rpc.StartListening();
            return rpc;
        });
    }
    protected virtual void SetupJsonRpc(Func<Stream, Stream, object> rpcTargetFactory)
    {
        (var clientStream, var serverStream) = FullDuplexStream.CreatePair();
        JsonRpc rpc = new(clientStream, serverStream);
        var rpcTarget = rpcTargetFactory(clientStream, serverStream);
        SetupJsonRpc(rpc, rpcTarget);
    }
}
