using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ReduxLib.Configuration;

/// <summary>
/// Represents a section of a configuration file.
/// </summary>
[PublicAPI]
public interface IConfigSection
{
    /// <summary>
    /// The name of the section.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The localization key for the section
    /// </summary>
    string? LocalizationKey { get; }

    /// <summary>
    /// Gets the <see cref="IConfigEntry" /> with the specified key.
    /// </summary>
    /// <param name="key">Key of the entry.</param>
    IConfigEntry this[string key] { get; }

    /// <summary>
    /// Binds a new <see cref="IConfigEntry" /> to the specified key within this section.
    /// </summary>
    /// <param name="key">Key of the entry.</param>
    /// <param name="defaultValue">Default value of the entry.</param>
    /// <param name="description">Description of the entry.</param>
    /// <param name="constraint">The initial constraint of the entry.</param>
    /// <typeparam name="T">Type of the entry.</typeparam>
    /// <returns>The bound <see cref="IConfigEntry" />.</returns>
    IConfigEntry Bind<T>(string key, T? defaultValue = default, string description = "", IValueConstraint? constraint = null)
        => BindEntry(typeof(T), key, defaultValue, description, constraint);

    /// <summary>
    /// Binds a new <see cref="IConfigEntry" /> to the specified key within this section, with full
    /// metadata including localization keys for the settings menu.
    /// </summary>
    /// <param name="key">Key of the entry.</param>
    /// <param name="defaultValue">Default value of the entry.</param>
    /// <param name="description">Description of the entry.</param>
    /// <param name="constraint">The initial constraint of the entry.</param>
    /// <param name="nameLocalizationKey">Localization key for the entry's display name.</param>
    /// <param name="descriptionLocalizationKey">Localization key for the entry's description.</param>
    /// <typeparam name="T">Type of the entry.</typeparam>
    /// <returns>The bound <see cref="IConfigEntry" />.</returns>
    IConfigEntry BindEntry<T>(
        string key,
        T? defaultValue = default,
        string description = "",
        IValueConstraint? constraint = null,
        string? nameLocalizationKey = null,
        string? descriptionLocalizationKey = null,
        IEnumerable<string>? tags = null
    ) => BindEntry(typeof(T), key, defaultValue, description, constraint, nameLocalizationKey, descriptionLocalizationKey, tags);

    /// <summary>
    /// Binds a new <see cref="IConfigEntry" /> whose value type is only known at runtime. The generic
    /// overloads delegate here.
    /// </summary>
    /// <param name="valueType">The C# type the entry stores.</param>
    /// <param name="key">Key of the entry.</param>
    /// <param name="defaultValue">Default value of the entry.</param>
    /// <param name="description">Description of the entry.</param>
    /// <param name="constraint">The initial constraint of the entry.</param>
    /// <param name="nameLocalizationKey">Localization key for the entry's display name.</param>
    /// <param name="descriptionLocalizationKey">Localization key for the entry's description.</param>
    /// <returns>The bound <see cref="IConfigEntry" />.</returns>
    IConfigEntry BindEntry(
        Type valueType,
        string key,
        object? defaultValue = null,
        string description = "",
        IValueConstraint? constraint = null,
        string? nameLocalizationKey = null,
        string? descriptionLocalizationKey = null,
        IEnumerable<string>? tags = null
    );

    /// <summary>
    /// All keys defined in this section.
    /// </summary>
    IReadOnlyList<string> Keys { get; }

    /// <summary>
    /// Resets every entry in this section to its default value.
    /// </summary>
    void Reset()
    {
        foreach (var key in Keys)
        {
            this[key].Value = this[key].Default;
        }
    }
}
