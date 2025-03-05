using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Abstractions.Models;

public interface IMessage
{
    [Required]
    [Description("The method indicating the type of message.")]
    string Method { get; }
}

public interface IParams
{
}

public interface IMessage<TParams> : IMessage
{
    [Required]
    [Description("The params object to supply to the request")]
    TParams Params { get; init; }
}

// Base Request Type
public interface IRequest : IMessage
{
}

public interface IRequest<TParams> : IMessage<TParams>
{
}

public abstract record BaseRequest(string Method) : IRequest
{
    [Required]
    [Description("The method to request.")]
    public string Method { get; private init; } = Method;
}

public abstract record BaseRequest<TParams>(
    string Method,
    TParams Params)
    : BaseRequest(Method), IRequest<TParams>
{
    [Description("The parameters to supply to the request.")]
    public virtual TParams Params { get; init; } = Params;
}

// InitializeRequest
public record InitializeRequest(
    InitializeRequest.Parameters @Params)
    : BaseRequest<InitializeRequest.Parameters>(MethodName, @Params),
        IClientRequest
{
    public const string MethodName = "initialize";
    public record Parameters : IParams
    {
        [Required]
        public ClientCapabilities Capabilities { get; init; } = new();

        [Required]
        public Implementation ClientInfo { get; init; } = new();

        [Required]
        [Description("The protocol version supported by the client.")]
        public string ProtocolVersion { get; init; } = "0.0.1";
    }
}

// PingRequest
public record PingRequest() : BaseRequest(MethodName), IClientRequest
{
    public const string MethodName = "ping";
}

// ListRootsRequest
public record ListRootsRequest(
    ListRootsRequest.Parameters Params)
    : BaseRequest<ListRootsRequest.Parameters>(MethodName, Params),
        IClientRequest
{
    public const string MethodName = "roots/list";
    public record Parameters : IParams
    {
        [Description("Optional metadata for the list roots request.")]
        public Dictionary<string, object>? Meta { get; init; }
    }
}

public abstract record PaginatedRequest(string Method) : BaseRequest(Method)
{
    [Description("The pagination cursor for the request.")]
    public string? Cursor { get; init; }
}

public abstract record PaginatedRequest<TParams>(
    string Method, TParams @Params)
    : PaginatedRequest(Method), IRequest<TParams>
{
    [Description("The parameters to supply to the request.")]
    public virtual TParams Params { get; init; } = @Params;
}

// /* Resources */
// /**
//  * Sent from the client to request a list of resources the server has.
//  */
// export interface ListResourcesRequest extends PaginatedRequest {
//   method: "resources/list";
// }
public record ListResourcesRequest()
    : PaginatedRequest(MethodName), IClientRequest
{
    public const string MethodName = "resources/list";
}

public record ListToolsRequest(
    ListToolsRequest.Parameters @Params)
    : PaginatedRequest<ListToolsRequest.Parameters>(MethodName, @Params)
{
    public const string MethodName = "tools/list";
    public record Parameters;
}

    // CallToolRequest
public record CallToolRequest(
    CallToolRequest.Parameters @Params)
    : BaseRequest<CallToolRequest.Parameters>(MethodName, @Params),
        IClientRequest
{
    public const string MethodName = "tools/call";
    public record Parameters
    {
        [Required]
        [Description("The name of the tool to be invoked.")]
        public string Name { get; init; } = string.Empty;

        [Description("The arguments required for invoking the tool.")]
        public Dictionary<string, object>? Arguments { get; init; }
    }
}

// /**
//  * Sent from the client to the server, to read a specific resource URI.
//  */
// export interface ReadResourceRequest extends Request {
//   method: "resources/read";
//   params: {
//     /**
//      * The URI of the resource to read. The URI can use any protocol; it is up to the server how to interpret it.
//      *
//      * @format uri
//      */
//     uri: string;
//   };
// }
public record ReadResourceRequest(
    ReadResourceRequest.Parameters @Params)
    : BaseRequest<ReadResourceRequest.Parameters>(MethodName, @Params),
        IClientRequest
{
    public const string MethodName = "resources/read";
    public record Parameters
    {
        [Required]
        [Description("The URI of the resource to read.")]
        public required Uri Uri { get; init; }
    }
}

// /**
//  * Sent from the client to request a list of resource templates the server has.
//  */
// export interface ListResourceTemplatesRequest extends PaginatedRequest {
//   method: "resources/templates/list";
// }
public record ListResourceTemplatesRequest()
    : BaseRequest(MethodName)
{
    public const string MethodName = "resources/templates/list";
}


// SubscribeRequest
public record SubscribeRequest(
    SubscribeRequest.Parameters @Params)
    : BaseRequest<SubscribeRequest.Parameters>(MethodName, @Params)
{
    public const string MethodName = "resources/subscribe";
    public record Parameters
    {
        [Required]
        [Description("The URI of the resource to subscribe to.")]
        public required Uri Uri { get; init; }
    }
}

// UnsubscribeRequest
public record UnsubscribeRequest(
    UnsubscribeRequest.Parameters @Params)
    : BaseRequest<UnsubscribeRequest.Parameters>(MethodName, @Params)
{
    public const string MethodName = "resources/unsubscribe";
    public record Parameters
    {
        [Required]
        [Description("The URI of the resource to unsubscribe from.")]
        public required Uri Uri { get; init; }
    }
}

// Base Client Request Interface
public interface IClientRequest : IRequest
{
}

// CompleteRequest
public record CompleteRequest(
    CompleteRequest.Parameters @Params)
    : BaseRequest<CompleteRequest.Parameters>(MethodName, Params),
        IClientRequest
{
    public const string MethodName = "completion/complete";

    public record Parameters
    {
        [Required]
        public CompletionArgument Argument { get; init; } = new CompletionArgument();

        [Required]
        public required CompletionReference Ref { get; init; }
    }

    public record CompletionArgument
    {
        [Required]
        [Description("The name of the argument.")]
        public string Name { get; init; } = string.Empty;

        [Required]
        [Description("The value of the argument.")]
        public string Value { get; init; } = string.Empty;
    }

    public abstract record CompletionReference
    {
        [Required]
        [Description("The type of the reference.")]
        public abstract string Type { get; init; }
    }

    public record PromptReference : CompletionReference
    {
        [Required]
        [Description("The name of the prompt or template.")]
        public string Name { get; init; } = string.Empty;

        [Required]
        [Description("The type of the reference.")]
        public override string Type { get; init; } = "ref/prompt";
    }

    public record ResourceReference : CompletionReference
    {
        [Required]
        [Description("The URI or URI template of the resource.")]
        public string Uri { get; init; } = string.Empty;

        [Required]
        [Description("The type of the reference.")]
        public override string Type { get; init; } = "ref/resource";
    }
}

public record CreateMessageRequest(CreateMessageRequest.Parameters Params)
    : BaseRequest<CreateMessageRequest.Parameters>(MethodName, Params)
{
    public const string MethodName = "messages/create";
    public record Parameters : IParams
    {
        public List<SamplingMessage> Messages { get; init; } = [];
        public ModelPreferences? ModelPreferences { get; init; }
        public string? SystemPrompt { get; init; }
        public IncludeContext? IncludeContext { get; init; }
        public double? Temperature { get; init; }
        public int MaxTokens { get; init; }
        public List<string>? StopSequences { get; init; }
        public Dictionary<string, object>? Metadata { get; init; }
    }
}


// /**
//  * The server's preferences for model selection, requested of the client during sampling.
//  *
//  * Because LLMs can vary along multiple dimensions, choosing the "best" model is
//  * rarely straightforward.  Different models excel in different areas—some are
//  * faster but less capable, others are more capable but more expensive, and so
//  * on. This interface allows servers to express their priorities across multiple
//  * dimensions to help clients make an appropriate selection for their use case.
//  *
//  * These preferences are always advisory. The client MAY ignore them. It is also
//  * up to the client to decide how to interpret these preferences and how to
//  * balance them against other considerations.
//  */
// export interface ModelPreferences {
//   /**
//    * Optional hints to use for model selection.
//    *
//    * If multiple hints are specified, the client MUST evaluate them in order
//    * (such that the first match is taken).
//    *
//    * The client SHOULD prioritize these hints over the numeric priorities, but
//    * MAY still use the priorities to select from ambiguous matches.
//    */
//   hints?: ModelHint[];

//   /**
//    * How much to prioritize cost when selecting a model. A value of 0 means cost
//    * is not important, while a value of 1 means cost is the most important
//    * factor.
//    *
//    * @TJS-type number
//    * @minimum 0
//    * @maximum 1
//    */
//   costPriority?: number;

//   /**
//    * How much to prioritize sampling speed (latency) when selecting a model. A
//    * value of 0 means speed is not important, while a value of 1 means speed is
//    * the most important factor.
//    *
//    * @TJS-type number
//    * @minimum 0
//    * @maximum 1
//    */
//   speedPriority?: number;

//   /**
//    * How much to prioritize intelligence and capabilities when selecting a
//    * model. A value of 0 means intelligence is not important, while a value of 1
//    * means intelligence is the most important factor.
//    *
//    * @TJS-type number
//    * @minimum 0
//    * @maximum 1
//    */
//   intelligencePriority?: number;
// }
public record ModelPreferences
{
    [Description("Optional hints to use for model selection.")]
    public List<ModelHint>? Hints { get; init; }
    [Description("How much to prioritize cost when selecting a model.")]
    [Range(0, 1)]
    public double? CostPriority { get; init; }
    [Description("How much to prioritize sampling speed (latency) when selecting a model.")]
    [Range(0, 1)]
    public double? SpeedPriority { get; init; }
    [Description("How much to prioritize intelligence and capabilities when selecting a model.")]
    [Range(0, 1)]
    public double? IntelligencePriority { get; init; }
}

// /**
//  * Hints to use for model selection.
//  *
//  * Keys not declared here are currently left unspecified by the spec and are up
//  * to the client to interpret.
//  */
// export interface ModelHint {
//   /**
//    * A hint for a model name.
//    *
//    * The client SHOULD treat this as a substring of a model name; for example:
//    *  - `claude-3-5-sonnet` should match `claude-3-5-sonnet-20241022`
//    *  - `sonnet` should match `claude-3-5-sonnet-20241022`, `claude-3-sonnet-20240229`, etc.
//    *  - `claude` should match any Claude model
//    *
//    * The client MAY also map the string to a different provider's model name or a different model family, as long as it fills a similar niche; for example:
//    *  - `gemini-1.5-flash` could match `claude-3-haiku-20240307`
//    */
//   name?: string;
// }
public record ModelHint
{
    public string? Name { get; init; }
}

public enum IncludeContext
{
    None,
    ThisServer,
    AllServers
}

//     export interface SetLevelRequest extends Request {
//   method: "logging/setLevel";
//   params: {
//     /**
//      * The level of logging that the client wants to receive from the server. The server should send all logs at this level and higher (i.e., more severe) to the client as notifications/logging/message.
//      */
//     level: LoggingLevel;
//   };
// }
public record SetLevelRequest(
    SetLevelRequest.Parameters @Params)
    : BaseRequest<SetLevelRequest.Parameters>(MethodName, @Params)
{
    public const string MethodName = "logging/setLevel";
    public record Parameters
    {
        [Required]
        [Description("The level of logging that the client wants to receive from the server.")]
        public LoggingLevel Level { get; init; }
    }
}

public record GetPromptRequest(
    GetPromptRequest.Parameters @Params)
    : BaseRequest<GetPromptRequest.Parameters>(MethodName, @Params)
{
    public const string MethodName = "prompts/get";
    public record Parameters
    {
        [Required]
        [Description("The name of the prompt or prompt template.")]
        public string Name { get; init; } = string.Empty;

        [Description("Arguments to use for templating the prompt.")]
        public Dictionary<string, string>? Arguments { get; init; }
    }
}

public record ListPromptsRequest() : BaseRequest(MethodName)
{
    public const string MethodName = "prompts/list";
}
