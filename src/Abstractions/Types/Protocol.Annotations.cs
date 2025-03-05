using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Abstractions.Models;

// Annotated Base Type
public interface IAnnotated
{
    [Description("Annotations for the object.")]
    Annotations? Annotations { get; set; }
}

public interface IResource : IAnnotated
{
    [Required]
    [Description("The URI of this resource.")]
    public Uri Uri { get; init; }

    [Required]
    [Description("A human-readable name for this resource.")]
    public string Name { get; init; }

    [Description("A description of what this resource represents.")]
    public string? Description { get; init; }

    [Description("The MIME type of this resource, if known.")]
    public string? MimeType { get; init; }
}


// Annotations
public record Annotations
{
    [Description("The audience this object is intended for.")]
    public List<Role>? Audience { get; init; }

    [Range(0, 1, ErrorMessage = "Priority must be between 0 and 1.")]
    [Description("The priority of the object, where 1 is most important and 0 is least important.")]
    public float? Priority { get; init; }
}

// AnnotatedResource
public record AnnotatedResource : IAnnotated
{
    [Required]
    [Description("The URI of the resource.")]
    public Uri Uri { get; init; } = new Uri("http://example.com");

    [Description("Annotations for the resource.")]
    public Annotations? Annotations { get; set; }
}

// AnnotatedContent
public record AnnotatedContent : IAnnotated
{
    [Required]
    [Description("The content of the object.")]
    public string Content { get; init; } = string.Empty;

    [Description("Annotations for the content.")]
    public Annotations? Annotations { get; set; }
}

// export interface ResourceTemplate extends Annotated {
//   /**
//    * A URI template (according to RFC 6570) that can be used to construct resource URIs.
//    *
//    * @format uri-template
//    */
//   uriTemplate: string;

//   /**
//    * A human-readable name for the type of resource this template refers to.
//    *
//    * This can be used by clients to populate UI elements.
//    */
//   name: string;

//   /**
//    * A description of what this template is for.
//    *
//    * This can be used by clients to improve the LLM's understanding of available resources. It can be thought of like a "hint" to the model.
//    */
//   description?: string;

//   /**
//    * The MIME type for all resources that match this template. This should only be included if all resources matching this template have the same type.
//    */
//   mimeType?: string;
// }

public record ResourceTemplate : IAnnotated
{
    [Required]
    [Description("A URI template (according to RFC 6570) that can be used to construct resource URIs.")]
    [RegularExpression(@"\{.*\}", ErrorMessage = "URI template must contain at least one URI template variable.")]
    public string UriTemplate { get; init; } = string.Empty;

    [Required]
    [Description("A human-readable name for the type of resource this template refers to.")]
    public string Name { get; init; } = string.Empty;

    [Description("A description of what this template is for.")]
    public string? Description { get; init; }

    [Description("The MIME type for all resources that match this template. This should only be included if all resources matching this template have the same type.")]
    public string? MimeType { get; init; }

    [Description("Annotations for the resource template.")]
    public Annotations? Annotations { get; set; }
}
