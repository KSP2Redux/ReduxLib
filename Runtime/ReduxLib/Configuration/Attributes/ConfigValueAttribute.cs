using System;

namespace ReduxLib.Configuration.Attributes;

/// <summary>
/// A config value attribute, used for adding a property/field into the config file
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class ConfigValueAttribute : Attribute
{
    /// <summary>
    /// The name of the config property
    /// </summary>
    public string Name;
    /// <summary>
    /// The description of the config property
    /// </summary>
    public string Description;
    /// <summary>
    /// The localization key for the name of the config property
    /// </summary>
    public string? NameLocalizationKey;
    /// <summary>
    /// The localization key for the description of the config property
    /// </summary>
    public string? DescriptionLocalizationKey;

    /// <summary>
    /// Creates a new config value attribute
    /// </summary>
    /// <param name="name">The name of the config value</param>
    /// <param name="description">The description of the config value</param>
    /// <param name="nameLoc">The localization key for the name of the config value</param>
    /// <param name="descLoc">The localization key for the description of the config value</param>
    public ConfigValueAttribute(string name, string description, string? nameLoc = null, string? descLoc = null)
    {
        Name = name;
        Description = description;
        NameLocalizationKey = nameLoc;
        DescriptionLocalizationKey = descLoc;
    }
}