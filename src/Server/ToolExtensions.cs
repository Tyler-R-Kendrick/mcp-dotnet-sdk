using System.ComponentModel;
using System.Reflection;
using Abstractions.Models;

namespace Server;

public static class ToolExtensions
{
    public static string? GetDescription(this MethodInfo methodInfo)
        => methodInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;

    public static string? GetDescription(this ParameterInfo parameterInfo)
        => parameterInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;

    public static Dictionary<string, object> ToDictionary(
        this ParameterInfo[] parameters) => parameters
        .ToDictionary(p => p.Name!, p => (object)new
        {
            Type = p.ParameterType.Name,
            Description = p.GetDescription(),
        });

    public static Tool ToTool(this Delegate implementation)
    {
        var method = implementation.Method;
        var parameters = method.GetParameters();
        var required = parameters
            .Where(p => !p.HasDefaultValue)
            .Select(p => p.Name)
            .OfType<string>()
            .ToArray();
        return new()
        {
            Name = method.Name,
            Description = method.GetDescription(),
            InputSchema = new()
            {
                Type = "object",
                Properties = parameters.ToDictionary(),
                Required = required,
            },
        };
    }
}
