
using Abstractions.Models;

namespace Server;


internal partial class Server
{
    protected virtual async Task<CreateMessageResult> HandleRequestAsync(
        CreateMessageRequest request,
        CancellationToken token) => request.Method switch
        {
            "callTool" => await HandleCallToolAsync(request, token),
            
            _ => throw new NotImplementedException(),
        };

    private async Task<CreateMessageResult> HandleCallToolAsync(
        CreateMessageRequest request,
        CancellationToken token)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }
}