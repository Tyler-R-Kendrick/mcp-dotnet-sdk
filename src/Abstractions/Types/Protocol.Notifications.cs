
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Abstractions.Models;

    // Base Notification Type
    public interface INotification
    {
        [Required]
        [Description("The method indicating the type of notification.")]
        string Method { get; init; }
    }
    
    // CancelledNotification
    public record CancelledNotification : INotification, IClientNotification, IServerNotificaiton
    {
        [Required]
        [Description("The method indicating a cancelled notification.")]
        public string Method { get; init; } = "notifications/cancelled";

        [Required]
        public Parameters Params { get; init; } = new Parameters();
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
    public record InitializedNotification : INotification, IClientNotification
    {
        [Required]
        [Description("The method indicating an initialized notification.")]
        public string Method { get; init; } = "notifications/initialized";

        [Description("Additional metadata for the notification.")]
        public Parameters Params { get; init; } = new Parameters();
        public record Parameters
        {
            [Description("Reserved parameter for attaching additional metadata.")]
            public Dictionary<string, object>? Meta { get; init; }
        }
    }

    // ProgressNotification
    public record ProgressNotification : INotification, IClientNotification, IServerNotificaiton
    {
        [Required]
        [Description("The method indicating a progress notification.")]
        public string Method { get; init; } = "notifications/progress";

        [Required]
        public Parameters Params { get; init; } = new Parameters();
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
    public record RootsListChangedNotification : INotification, IClientNotification
    {
        [Required]
        [Description("The method indicating a roots list changed notification.")]
        public string Method { get; init; } = "notifications/roots/list_changed";

        [Description("Additional metadata for the roots list changed notification.")]
        public Parameters Params { get; init; } = new Parameters();
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


// export type ServerNotification =
//   | CancelledNotification
//   | ProgressNotification
//   | LoggingMessageNotification
//   | ResourceUpdatedNotification
//   | ResourceListChangedNotification
//   | ToolListChangedNotification
//   | PromptListChangedNotification;
    public interface IServerNotificaiton : INotification
    {
    }

    // ResourceListChangedNotification
    public record ResourceListChangedNotification : INotification
    {
        [Required]
        [Description("The method indicating a resource list changed notification.")]
        public string Method { get; init; } = "notifications/resources/list_changed";

        [Description("Additional metadata for the resource list changed notification.")]
        public Parameters Params { get; init; } = new Parameters();
        public record Parameters
        {
            [Description("Reserved parameter for attaching additional metadata.")]
            public Dictionary<string, object>? Meta { get; init; }
        }
    }

    // ResourceUpdatedNotification
    public record ResourceUpdatedNotification : INotification, IServerNotificaiton
    {
        [Required]
        [Description("The method indicating a resource updated notification.")]
        public string Method { get; init; } = "notifications/resources/updated";

        [Required]
        public Parameters Params { get; init; } = new Parameters();
        public record Parameters
        {
            [Required]
            [Description("The URI of the updated resource.")]
            public Uri Uri { get; init; } = new Uri("http://example.com");
        }
    }

    // PromptListChangedNotification
    public record PromptListChangedNotification : INotification, IServerNotificaiton
    {
        [Required]
        [Description("The method indicating a prompt list changed notification.")]
        public string Method { get; init; } = "notifications/prompts/list_changed";

        [Description("Additional metadata for the prompt list changed notification.")]
        public Parameters Params { get; init; } = new Parameters();
        public record Parameters
        {
            [Description("Reserved parameter for attaching additional metadata.")]
            public Dictionary<string, object>? Meta { get; init; }
        }
    }

    // ToolListChangedNotification
    public record ToolListChangedNotification : INotification, IServerNotificaiton
    {
        [Required]
        [Description("The method indicating a tool list changed notification.")]
        public string Method { get; init; } = "notifications/tools/list_changed";

        [Description("Additional metadata for the tool list changed notification.")]
        public Parameters Params { get; init; } = new Parameters();
        public record Parameters
        {
            [Description("Reserved parameter for attaching additional metadata.")]
            public Dictionary<string, object>? Meta { get; init; }
        }
    }

    // LoggingMessageNotification
    public record LoggingMessageNotification
    {
        [Required]
        [Description("The method indicating a logging message notification.")]
        public string Method { get; init; } = "notifications/message";

        [Required]
        public Parameters Params { get; init; } = new Parameters();
        public record Parameters
        {
            [Required]
            [Description("The severity level of the log message.")]
            public LoggingLevel Level { get; init; }

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
