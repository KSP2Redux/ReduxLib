using System;

namespace ReduxLib.Configuration.Attributes;

/// <summary>
/// Starts a section for the config file, if it is a previously defined section, everything will go there
/// Somewhat similar to Unity's [Header(...)] attribute
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class ConfigSectionAttribute : Attribute
{
    /// <summary>
    /// The name of the section
    /// </summary>
    public string Name;
    /// <summary>
    /// The localization key of the section
    /// </summary>
    public string? LocalizationKey;

    /// <summary>
    /// Creates a new ConfigSection attribute
    /// </summary>
    /// <param name="name">The name of the section</param>
    /// <param name="loc">The optional localization key for the section</param>
    public ConfigSectionAttribute(string name, string? loc = null)
    {
        Name = name;
        LocalizationKey = loc;
    }
}