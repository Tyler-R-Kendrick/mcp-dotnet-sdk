using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Abstractions.Models;

// Base Notification Type
public interface INotification : IMessage
{
}

public interface INotification<TParams> : INotification
{
    [Required]
    TParams Params { get; }
}

public abstract record BaseNotification<TParams>(string methodName, TParams @params)
    : INotification<TParams>
{
    [Required]
    [Description("The method indicating a cancelled notification.")]
    public string Method { get; init; } = methodName;
    public virtual TParams Params { get; init; } = @params;
}

// CancelledNotification
public record CancelledNotification(CancelledNotification.Parameters @Params)
    : BaseNotification<CancelledNotification.Parameters>(MethodName,@Params),
        IClientNotification, IServerNotificaiton
{
    public const string MethodName = "notifications/cancelled";
    public record Parameters
    {
        [Required]
        [Description("The ID of the request to cancel.")]
        public string RequestId { get; init; } = string.Empty;

        [Description("An optional string describing the reason for the cancellation.")]
        public string? Reason { get; init; }
    }
}

// InitializedNotification
public record InitializedNotification(InitializedNotification.Parameters @Params)
    : BaseNotification<InitializedNotification.Parameters>(MethodName, @Params),
    IClientNotification
{
    public const string MethodName = "notifications/initialized";
    public record Parameters
    {
        [Description("Reserved parameter for attaching additional metadata.")]
        public Dictionary<string, object>? Meta { get; init; }
    }
}

// ProgressNotification
public record ProgressNotification(ProgressNotification.Parameters @Params)
    : BaseNotification<ProgressNotification.Parameters>(MethodName, @Params),
        IClientNotification, IServerNotificaiton
{
    public const string MethodName = "notifications/progress";
    public record Parameters
    {
        [Required]
        [Description("The progress value thus far.")]
        public double Progress { get; init; }

        [Required]
        [Description("The progress token to associate this notification with its request.")]
        public ProgressToken ProgressToken { get; init; } = new ProgressToken();

        [Description("Total number of items to process, if known.")]
        public double? Total { get; init; }
    }
}

public record ProgressToken
{

}

// RootsListChangedNotification
public record RootsListChangedNotification(RootsListChangedNotification.Parameters @Params)
    : BaseNotification<RootsListChangedNotification.Parameters>(MethodName, @Params),
        IClientNotification
{
    public const string MethodName = "notifications/roots/list_changed";
    public record Parameters
    {
        [Description("Reserved parameter for attaching additional metadata.")]
        public Dictionary<string, object>? Meta { get; init; }
    }
}

// ClientNotification
public interface IClientNotification : INotification
{
}

public interface IServerNotificaiton : INotification
{
}

// ResourceListChangedNotification
public record ResourceListChangedNotification(
    ResourceListChangedNotification.Parameters @Params)
    : BaseNotification<ResourceListChangedNotification.Parameters>(MethodName, @Params),
        IServerNotificaiton
{
    public const string MethodName = "notifications/resources/list_changed";
    public record Parameters
    {
        [Description("Reserved parameter for attaching additional metadata.")]
        public Dictionary<string, object>? Meta { get; init; }
    }
}

// ResourceUpdatedNotification
public record ResourceUpdatedNotification(
    ResourceUpdatedNotification.Parameters @Params)
    : BaseNotification<ResourceUpdatedNotification.Parameters>(MethodName, @Params),
        IServerNotificaiton
{
    public const string MethodName = "notifications/resources/updated";
    public record Parameters
    {
        [Required]
        [Description("The URI of the updated resource.")]
        public required Uri Uri { get; init; }
    }
}

// PromptListChangedNotification
public record PromptListChangedNotification(
    PromptListChangedNotification.Parameters @Params)
    : BaseNotification<PromptListChangedNotification.Parameters>(MethodName, @Params),
        IServerNotificaiton
{
    public const string MethodName = "notifications/prompts/list_changed";
    public record Parameters
    {
        [Description("Reserved parameter for attaching additional metadata.")]
        public Dictionary<string, object>? Meta { get; init; }
    }
}

// ToolListChangedNotification
public record ToolListChangedNotification(
    ToolListChangedNotification.Parameters @Params)
    : BaseNotification<ToolListChangedNotification.Parameters>(MethodName, @Params),
        IServerNotificaiton
{
    public const string MethodName = "notifications/tools/list_changed";
    public record Parameters
    {
        [Description("Reserved parameter for attaching additional metadata.")]
        public Dictionary<string, object>? Meta { get; init; }
    }
}

// LoggingMessageNotification
public record LoggingMessageNotification(
    LoggingMessageNotification.Parameters @Params)
    : BaseNotification<LoggingMessageNotification.Parameters>(MethodName, @Params),
        IServerNotificaiton
{
    public const string MethodName = "notifications/message";
    public record Parameters
    {
        [Required]
        [Description("The severity level of the log message.")]
        public LoggingLevel Level { get; init; } = LoggingLevel.Critical;

        [Required]
        [Description("The actual data to be logged.")]
        public object Data { get; init; } = new();

        [Description("An optional name of the logger issuing the message.")]
        public string? Logger { get; init; }
    }
}

public enum LoggingLevel
{
    [Description("An alert condition requiring immediate action.")]
    Alert,

    [Description("Critical conditions requiring immediate resolution.")]
    Critical,

    [Description("Debug-level messages used for troubleshooting.")]
    Debug,

    [Description("Emergency-level messages requiring immediate attention.")]
    Emergency,

    [Description("Error conditions.")]
    Error,

    [Description("Informational messages.")]
    Info,

    [Description("Normal but significant conditions.")]
    Notice,

    [Description("Warning conditions.")]
    Warning
}
