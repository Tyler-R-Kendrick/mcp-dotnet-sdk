using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Abstractions.Models;
public enum Role
{
    [Description("Assistant role in the conversation.")]
    Assistant,

    [Description("User role in the conversation.")]
    User
}

public record Root
{
    [Required]
    [Description("The URI identifying the root. This *must* start with file:// for now.")]
    public required Uri Uri { get; init; }

    [Description("An optional name for the root.")]
    public string? Name { get; init; }
}

public record Implementation
{
    [Required]
    [Description("The name of the implementation.")]
    public string Name { get; init; } = string.Empty;

    [Required]
    [Description("The version of the implementation.")]
    public string Version { get; init; } = string.Empty;
}

    public record ClientCapabilities
    {
        [Description("Experimental capabilities supported by the client.")]
        public Dictionary<string, object>? Experimental { get; init; }

        [Description("Capabilities related to listing roots.")]
        public RootsCapabilities? Roots { get; init; }

        [Description("Present if the client supports sampling from an LLM.")]
        public Dictionary<string, object>? Sampling { get; init; }
    }

    public record RootsCapabilities
    {
        [Description("Whether the client supports notifications for changes to the roots list.")]
        public bool? ListChanged { get; init; }
    }

    public record SamplingMessage
    {
        [Required]
        [Description("The role of the sender.")]
        public Role Role { get; init; }

        [Required]
        [Description("The content of the message.")]
        public ISamplingContent? Content { get; init; }
    }

    public record PromptMessage
    {
        [Required]
        [Description("The role of the sender.")]
        public Role Role { get; init; }

        [Required]
        [Description("The content of the message.")]
        public IContent? Content { get; init; }
    }
    public record Tool
    {
        [Required]
        [Description("The name of the tool.")]
        public string Name { get; init; } = string.Empty;

        [Description("A human-readable description of the tool.")]
        public string? Description { get; init; }

        [Required]
        [Description("A JSON Schema object defining the expected parameters for the tool.")]
        public InputSchema InputSchema { get; init; } = new();
    }
    public record InputSchema
    {
        [Required]
        [Description("The type of the schema.")]
        public string Type { get; init; } = "object";

        [Description("The properties of the schema.")]
        public Dictionary<string, object>? Properties { get; init; } = [];

        [Description("The required properties of the schema.")]
        public string[]? Required { get; init; } = [];
    }

public record Prompt
{
    [Required]
    [Description("The name of the prompt.")]
    public string Name { get; init; } = string.Empty;

    [Description("A description of the prompt.")]
    public string? Description { get; init; }

    [Description("Arguments required for the prompt.")]
    public List<PromptArgument>? Arguments { get; init; }
}

public record PromptArgument
{
    [Required]
    [Description("The name of the argument.")]
    public string Name { get; init; } = string.Empty;

    [Description("A description of the argument.")]
    public string? Description { get; init; }

    [Description("Indicates if the argument is required.")]
    public bool? Required { get; init; }
}


/**
 * An incomplete set of error codes that may appear in JSON-RPC responses.
 */
public enum ErrorCode 
{
  // SDK error codes
  ConnectionClosed = -1,
  RequestTimeout = -2,

  // Standard JSON-RPC error codes
  ParseError = -32700,
  InvalidRequest = -32600,
  MethodNotFound = -32601,
  InvalidParams = -32602,
  InternalError = -32603,
}

