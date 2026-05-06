using JetBrains.Annotations;

namespace ReduxLib.Configuration;

/// <summary>
/// Represents a configuration file.
/// </summary>
[PublicAPI]
public interface IConfigFile
{
    /// <summary>
    /// Saves the configuration file.
    /// </summary>
    void Save();

    /// <summary>
    /// All sections in the configuration file, in declaration order. Also indexable by section name.
    /// </summary>
    ConfigSectionList Sections { get; }

    /// <summary>
    /// Gets the section with the given name, creating it if it doesn't exist yet.
    /// </summary>
    IConfigSection GetOrCreateSection(string name) => GetOrCreateSection(name, null);

    /// <summary>
    /// Gets the section with the given name, creating it if it doesn't exist yet.
    /// If the section is being created, <paramref name="localizationKey" /> is used as its
    /// localization key. If the section already exists, its existing localization key is preserved.
    /// </summary>
    IConfigSection GetOrCreateSection(string name, string? localizationKey);

    /// <summary>
    /// Gets the <see cref="IConfigEntry" /> with the specified section and key.
    /// </summary>
    /// <param name="section">Section of the entry.</param>
    /// <param name="key">Key of the entry.</param>
    IConfigEntry this[string section, string key] => Sections[section][key];

    /// <summary>
    /// Binds a new <see cref="IConfigEntry" /> to the specified section and key.
    /// </summary>
    /// <param name="section">Section of the entry.</param>
    /// <param name="key">Key of the entry.</param>
    /// <param name="defaultValue">Default value of the entry.</param>
    /// <param name="description">Description of the entry.</param>
    /// <param name="constraint">The initial constraint of the entry.</param>
    /// <typeparam name="T">Type of the entry.</typeparam>
    /// <returns>The bound <see cref="IConfigEntry" />.</returns>
    IConfigEntry Bind<T>(string section, string key, T? defaultValue = default, string description = "", IValueConstraint? constraint = null)
        => GetOrCreateSection(section).Bind(key, defaultValue, description, constraint);

    /// <summary>
    /// Resets every entry in every section to its default value.
    /// </summary>
    void Reset()
    {
        foreach (var section in Sections)
        {
            section.Reset();
        }
    }
}
