using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.UnityConverters.Configuration;
using UnityEngine;

namespace ReduxLib.Configuration;

/// <summary>
/// A config file that uses JSON to store its data.
/// </summary>
[PublicAPI]
public class JsonConfigFile : IConfigFile
{
    [CanBeNull] private JObject _previousConfigObject;

    /// <inheritdoc />
    public ConfigSectionList Sections { get; } = new();

    private readonly string _file;

    /// <summary>
    /// Creates a new JSON config file object.
    /// </summary>
    /// <param name="file">The file path to use.</param>
    public JsonConfigFile(string file)
    {
        // Use .cfg as this is going to have comments and that will be an issue
        if (File.Exists(file))
        {
            try
            {
                _previousConfigObject = JObject.Parse(File.ReadAllText(file));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in attempting to load previous config file at '{file}': {e}");
                // ignored
            }
        }

        _file = file;
    }

    /// <inheritdoc />
    public IConfigSection GetOrCreateSection(string name, string? localizationKey)
    {
        if (Sections.TryGet(name, out var existing))
        {
            return existing!;
        }

        JObject? slice = null;
        if (_previousConfigObject != null
            && _previousConfigObject.TryGetValue(name, out var token)
            && token is JObject obj)
        {
            slice = obj;
        }

        var section = new JsonConfigSection(this, name, slice, localizationKey);
        Sections.Add(section);
        return section;
    }

    /// <inheritdoc />
    public void Save()
    {
        var nonEmptySections = Sections
            .OfType<JsonConfigSection>()
            .Where(s => s.Entries.Count > 0)
            .ToList();
        if (nonEmptySections.Count == 0) return;

        var result = new StringBuilder();
        result.AppendLine("{");
        var hadPreviousSection = false;
        foreach (var section in nonEmptySections)
        {
            if (hadPreviousSection)
            {
                result.AppendLine(",");
            }
            section.WriteTo(result);
            hadPreviousSection = true;
        }
        result.AppendLine("\n}");
        File.WriteAllText(_file, result.ToString());
    }

    private static List<JsonConverter>? _defaultConverters;

    private static List<JsonConverter>? CreateDefaultConverters()
    {
        var method = Assembly.Load("Newtonsoft.Json.UnityConverters")
            .GetType("Newtonsoft.Json.UnityConverters.UnityConverterInitializer")
            .GetMethod("CreateConverters", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
        var parameters = new object[] { ScriptableObject.CreateInstance<UnityConvertersConfig>() };
        return (List<JsonConverter>)method.Invoke(null, parameters);
    }

    /// <summary>
    /// The default converters to use when serializing/deserializing JSON.
    /// </summary>
    public static List<JsonConverter>? DefaultConverters
    {
        get
        {
            if (_defaultConverters != null) return _defaultConverters;
            _defaultConverters = CreateDefaultConverters();
            _defaultConverters!.Add(new StringEnumConverter());

            return _defaultConverters;
        }
    }

    internal static bool DumpEntry(
        StringBuilder result,
        bool hadPreviousKey,
        KeyValuePair<string, JsonConfigEntry> entry
    )
    {
        if (hadPreviousKey)
        {
            result.AppendLine(",");
        }

        // result.AppendLine($"        // {entry.Value.Description}");
        if (entry.Value.Description != "")
        {
            var descriptionLines = entry.Value.Description.Split('\n').Select(x => x.TrimEnd());
            foreach (var line in descriptionLines)
            {
                result.AppendLine($"        // {line}");
            }
        }

        if (entry.Value.Constraint is IValueConstraint constraint)
        {
            var constraintLines = constraint.ConstraintDescription.Split('\n').Select(x => x.TrimEnd());
            foreach (var line in constraintLines)
            {
                result.AppendLine($"        // {line}");
            }
        }

        result.AppendLine($"        // Default: {entry.Value.Default}");

        var serialized = JsonConvert.SerializeObject(entry.Value.Value,Formatting.Indented,DefaultConverters.ToArray());
        var serializedLines = serialized.Split('\n').Select(x => x.TrimEnd()).ToArray();
        if (serializedLines.Length > 1)
        {
            result.AppendLine($"        \"{entry.Key.Replace("\"", "\\\"").Replace("\n", "\\\n")}\": ");
            for (var i = 0; i < serializedLines.Length; i++)
            {
                if (i != serializedLines.Length - 1)
                {
                    result.AppendLine($"        {serializedLines[i]}");
                }
                else
                {
                    result.Append($"        {serializedLines[i]}");
                }
            }
        }
        else
        {
            result.Append($"        \"{entry.Key.Replace("\"", "\\\"").Replace("\n", "\\\n")}\": {serializedLines[0]}");
        }

        return true;
    }
}
