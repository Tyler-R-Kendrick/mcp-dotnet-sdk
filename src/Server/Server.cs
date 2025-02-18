using Abstractions.Models;
using StreamJsonRpc;
using Microsoft.Extensions.AI;
using Polly;

namespace Server;

public static class RequestExtensions
{
    public static Task SendAsync<TParams>(
        this IRequest<TParams> request,
        JsonRpc transport,
        CancellationToken token = default)
        => transport.InvokeWithParameterObjectAsync(
            targetName: request.Method,
            argument: request.Params,
            cancellationToken: token);
}
internal partial class Server(
    IChatClient chatClient,
    JsonRpc transport,
    Implementation implementation,
    ClientCapabilities clientCapabilities,
    ListRootsResult listRootsResult,
    Dictionary<string, AIFunction> tools,
    string defaultModel,
    ResiliencePipeline pipeline,
    System.ComponentModel.TypeConverter converter)
    : IProtocol, IDisposable
{
    private async Task RequestAsync<TParams>(
        IRequest<TParams> request,
        CancellationToken token = default)
    {
        AssertCapability(request.Method);
        switch(request)
        {
            case CallToolRequest.MethodName:
                var callToolRequest = (CallToolRequest)request;
                var toolParams = callToolRequest.Params;
                var toolName = toolParams.Name;
                var toolArgs = (toolParams.Arguments ?? [])
                    .ToDictionary(x => x.Key, x => (object?)x.Value)
                    .ToArray();
                var tool = tools[toolName];
                await tool.InvokeAsync(toolArgs, token);
                break;
        }
    }

    private void AssertCapability(string method)
    {
        switch(method)
        {
            case CreateMessageRequest.MethodName
                when clientCapabilities.Sampling is null:
                    throw new Exception("Client does not support sampling (required for CreateMessage)");
            case ListRootsRequest.MethodName
                when clientCapabilities.Roots is null:
                    throw new Exception("Client does not support listing roots (required for ListRoots)");
            default: break;
        };
    }

    public async Task<CreateMessageResult> CreateMessageAsync(
        CreateMessageRequest.Parameters @params,
        CancellationToken token = default)
    {
        ChatRole ConvertRole(Role role) => role switch
        {
            Role.User => ChatRole.User,
            Role.Assistant => ChatRole.Assistant,
            _ => ChatRole.System
        };
        var messages = @params.Messages.Select(x => new ChatMessage(ConvertRole(x.Role), x.Content?.ToString()));
        ChatMessage systemPrompt = new(ChatRole.System, @params.SystemPrompt);

        IDictionary<string, object?> ConvertMetadata(Dictionary<string, object> metadata)
        {
            Dictionary<string, object?> result = [];
            foreach (var kvp in metadata)
            {
                result[kvp.Key] = kvp.Value;
            }
            return result;
        }
        //TODO: Implement conversion of @params to chat options
        ChatOptions ConvertOptions(CreateMessageRequest.Parameters @params) => new()
        {
            Temperature = @params.Temperature.HasValue ? (float?)@params.Temperature.Value : null,
            MaxOutputTokens = @params.MaxTokens,
            StopSequences = @params.StopSequences,
            AdditionalProperties = new(ConvertMetadata(@params.Metadata ?? []))
        };
        var chatResponse = await chatClient.CompleteAsync(
            chatMessages: [systemPrompt, ..messages],
            options: ConvertOptions(@params), 
            token);
        var messageContents = chatResponse.Message.Contents.Select(x => x.ToString());
        Abstractions.Models.TextContent responseContent = new()
        {
            Text = string.Join("", messageContents)
        };
        Role role = chatResponse.Message.Role switch
        {
            var r when r == ChatRole.User => Role.User,
            var r when r == ChatRole.Assistant => Role.Assistant,
            _ => throw new Exception("Invalid role: System not supported")
        };
        StopReason? ConvertReason(ChatFinishReason? finishReason) => finishReason switch
        {
            var x when x == ChatFinishReason.Stop => StopReason.StopSequence,
            var x when x == ChatFinishReason.Length => StopReason.MaxTokens,
            var x when x == ChatFinishReason.ToolCalls => StopReason.EndTurn,
            var x when x == ChatFinishReason.ContentFilter => StopReason.EndTurn,
            _ => null
        };
        return new CreateMessageResult
        {
            Content = responseContent,
            Model = chatResponse.ModelId ?? defaultModel,
            Role = role,
            StopReason = ConvertReason(chatResponse.FinishReason)
        };
    }

    public Task<ClientCapabilities> GetClientCapabilitiesAsync(
        CancellationToken token = default)
        => Task.FromResult(clientCapabilities);

    public Task<Implementation> GetClientVersionAsync(
        CancellationToken token = default)
        => Task.FromResult(implementation);

    public Task<ListRootsResult> ListRootsAsync(
        ListRootsRequest.Parameters? @params = null,
        CancellationToken token = default)
    {
        ListRootsRequest request = new(@params ?? new());
        var listRoots = listRootsResult;
        return Task.FromResult(listRoots);
    }

    [JsonRpcMethod("ping")]
    public Task<EmptyResult> PingAsync(CancellationToken token = default)
        => Task.FromResult(new EmptyResult());

    public Task RegisterCapabilitesAsync(
        ServerCapabilities capabilities,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        transport.Dispose();
        chatClient.Dispose();
    }
}
