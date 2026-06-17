using System;

namespace ReduxLib.Configuration.Attributes;

/// <summary>
/// Attaches a metadata tag to a config value, alongside <see cref="ConfigValueAttribute" />.
/// </summary>
/// <remarks>
/// Multiple tags are allowed. Tags are inert until some system polls the ones it cares about. There is no
/// tag-to-behavior registry.
/// </remarks>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public sealed class ConfigMetadataAttribute : Attribute
{
    /// <summary>
    /// The tag name.
    /// </summary>
    public string Tag { get; }

    /// <summary>
    /// Creates a new config metadata tag.
    /// </summary>
    /// <param name="tag">The tag name.</param>
    public ConfigMetadataAttribute(string tag)
    {
        Tag = tag;
    }
}
