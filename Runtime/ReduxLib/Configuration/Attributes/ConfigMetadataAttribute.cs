using System;

namespace ReduxLib.Configuration.Attributes;

/// <summary>
/// Attaches a metadata tag to a config value, alongside <see cref="ConfigValueAttribute" />. Multiple tags
/// are allowed. Tags are inert until some system polls the ones it cares about (there is no tag-to-behavior
/// registry).
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class ConfigMetadataAttribute : Attribute
{
    /// <summary>
    /// The tag name.
    /// </summary>
    public string Tag;

    /// <summary>
    /// Creates a new config metadata tag.
    /// </summary>
    /// <param name="tag">The tag name.</param>
    public ConfigMetadataAttribute(string tag)
    {
        Tag = tag;
    }
}
