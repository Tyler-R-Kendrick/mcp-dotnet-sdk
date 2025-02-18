
using Abstractions.Models;
using Microsoft.Extensions.AI;
using Polly;

namespace Server;

internal partial class Server
{
    private AIFunction GetTool(CallToolRequest.Parameters @params)
        => tools[@params.Name];

    private Dictionary<string, object?> GetToolArgs(CallToolRequest.Parameters @params)
        => (@params.Arguments ?? []).ToDictionary(x => x.Key, x => (object?)x.Value);

    private ValueTask<Outcome<IContent[]>> GetToolContentsAsync(
        CallToolRequest request,
        CancellationToken token = default)
    {
        var toolParams = request.Params;
        var toolArgs = GetToolArgs(toolParams).ToArray();
        var tool = GetTool(toolParams);
        var context = ResilienceContextPool.Shared.Get(token);
        return pipeline.ExecuteOutcomeAsync(
            async (context, state) =>
            {
                var response = await tool.InvokeAsync(toolArgs, token);
                List<IContent> contents = [];
                await foreach (var content in HandleResultsAsync(response))
                {
                    contents.Add(content);
                }
                return Outcome.FromResult(contents.ToArray());
            }, context, Array.Empty<IContent>());
    }

    private async IAsyncEnumerable<IContent> HandleResultsAsync(object? result)
    {
        //TODO: implement proper type conversion for tool responses to IContent implementations.
        //This should likely be done by an injectable/configurable type converter.
        switch (result)
        {
            case IAsyncEnumerable<IContent> enumerable:
                await foreach (var item in enumerable)
                {
                    yield return item;
                }
                break;

            case string text:
                yield return new Abstractions.Models.TextContent() { Text = text };
                break;

            case System.Collections.IEnumerable enumerable:
                foreach (var item in enumerable)
                {
                    await foreach (var subItem in HandleResultsAsync(item))
                    {
                        yield return subItem;
                    }
                }
                break;

            case IContent content:
                yield return content;
                break;

            case object target when target is not null and (int or long or float or double):
                yield return new Abstractions.Models.TextContent() { Text = target.ToString()! };
                break;

            default:
                var hasConversion = result != null
                    && converter.CanConvertFrom(result.GetType())
                    && converter.CanConvertTo(typeof(IContent));
                yield return hasConversion
                    ? (IContent)converter.ConvertTo(result, typeof(IContent))!
                    : throw new InvalidCastException($"Cannot cast {result?.GetType()} to {typeof(IContent)}");
                break;
        }
    }
    
    public async Task<CallToolResult> CallToolAsync(
        CallToolRequest request,
        CancellationToken token = default)
    {
        var outcome = await GetToolContentsAsync(request, token);
        return new()
        {
            Content = outcome.Result ?? [],
            IsError = outcome.Exception is not null,
        };
    }
}
