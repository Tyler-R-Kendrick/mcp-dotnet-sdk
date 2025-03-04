using StreamJsonRpc;

namespace Server;
using Models;

internal class DelegatedServerFactory(Func<JsonRpc> transportFactory)
{
    public DelegatingServer Create(
        OnCallToolAsync? callToolHandler = null,
        OnListToolsAsync? listToolsHandler = null,
        OnGetPromptAsync? getPromptHandler = null,
        OnListPromptsAsync? listPromptsHandler = null,
        OnCompleteAsync? completeHandler = null,
        OnReadResourceAsync? readResourceHandler = null,
        OnListResourcesAsync? listResourcesHandler = null)
    => new(transportFactory())
    {
        CallToolHandler = callToolHandler,
        ListToolsHandler = listToolsHandler,
        GetPromptHandler = getPromptHandler,
        ListPromptsHandler = listPromptsHandler,
        CompleteHandler = completeHandler,
        ReadResourceHandler = readResourceHandler,
        ListResourcesHandler = listResourcesHandler
    };
}
