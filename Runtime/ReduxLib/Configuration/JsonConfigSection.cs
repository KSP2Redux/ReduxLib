using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace ReduxLib.Configuration;

/// <summary>
/// A section within a <see cref="JsonConfigFile" />.
/// </summary>
[PublicAPI]
public class JsonConfigSection : IConfigSection
{
    private readonly JsonConfigFile _file;
    private readonly JObject? _previousSlice;

    internal readonly Dictionary<string, JsonConfigEntry> Entries = new();

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc/>
    public string? LocalizationKey { get; }

    internal JsonConfigSection(JsonConfigFile file, string name, JObject? previousSlice, string? localizationKey)
    {
        _file = file;
        Name = name;
        _previousSlice = previousSlice;
        LocalizationKey = localizationKey;
    }

    /// <inheritdoc />
    public IConfigEntry this[string key] => Entries[key];

    /// <inheritdoc />
    public IReadOnlyList<string> Keys => Entries.Keys.ToList();

    /// <inheritdoc />
    public IConfigEntry Bind<T>(string key, T? defaultValue = default, string description = "", IValueConstraint? constraint = null)
        => BindEntry(key, defaultValue, description, constraint);

    /// <inheritdoc />
    public IConfigEntry BindEntry<T>(
        string key,
        T? defaultValue = default,
        string description = "",
        IValueConstraint? constraint = null,
        string? nameLocalizationKey = null,
        string? descriptionLocalizationKey = null
    )
    {
        if (Entries.TryGetValue(key, out var existing))
        {
            return existing;
        }

        if (_previousSlice != null && _previousSlice.TryGetValue(key, out var token))
        {
            try
            {
                var previousValue = token.ToObject(typeof(T));
                Entries[key] = new JsonConfigEntry(_file, typeof(T), description, previousValue, constraint, nameLocalizationKey, descriptionLocalizationKey);
            }
            catch
            {
                Entries[key] = new JsonConfigEntry(_file, typeof(T), description, defaultValue, constraint, nameLocalizationKey, descriptionLocalizationKey);
            }
        }
        else
        {
            Entries[key] = new JsonConfigEntry(_file, typeof(T), description, defaultValue, constraint, nameLocalizationKey, descriptionLocalizationKey);
        }

        _file.Save();
        return Entries[key];
    }

    internal void WriteTo(StringBuilder result)
    {
        result.AppendLine($"    \"{Name.Replace("\"", "\\\"").Replace("\n", "\\\n")}\": {{");
        var hadPreviousKey = false;
        foreach (var entry in Entries)
        {
            hadPreviousKey = JsonConfigFile.DumpEntry(result, hadPreviousKey, entry);
        }
        result.Append("\n    }");
    }
}
