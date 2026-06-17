using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ReduxLib.Configuration;

/// <summary>
/// A config entry that is stored in a JSON file.
/// </summary>
[PublicAPI]
public class JsonConfigEntry : IConfigEntry
{
    private readonly JsonConfigFile _configFile;
    private object _value;
    private readonly HashSet<string> _tags;

    /// <summary>
    /// The callbacks that are invoked when the value of this entry changes.
    /// </summary>
    public event Action<object, object>? Callbacks;

    /// <summary>
    /// Creates a new config entry.
    /// </summary>
    /// <param name="configFile">Config file that this entry belongs to.</param>
    /// <param name="type">Type of the value.</param>
    /// <param name="description">Description of the value.</param>
    /// <param name="value">Value of the entry.</param>
    /// <param name="constraint">Constraint of the value.</param>
    /// <param name="nameLocalizationKey">Localization key for the entry's display name.</param>
    /// <param name="descriptionLocalizationKey">Localization key for the entry's description.</param>
    /// <param name="tags">Metadata tags declared on the entry.</param>
    public JsonConfigEntry(
        JsonConfigFile configFile,
        Type type,
        string description,
        object value, IValueConstraint? constraint = null,
        string? nameLocalizationKey = null,
        string? descriptionLocalizationKey = null,
        IEnumerable<string>? tags = null
    )
    {
        _configFile = configFile;
        _value = value;
        NameLocalizationKey = nameLocalizationKey;
        DescriptionLocalizationKey = descriptionLocalizationKey;
        Default = value;
        Constraint = constraint;
        Description = description;
        ValueType = type;
        _tags = tags != null ? new HashSet<string>(tags) : new HashSet<string>();
    }

    /// <inheritdoc />
    public string? NameLocalizationKey { get; }

    /// <inheritdoc />
    public string? DescriptionLocalizationKey { get; }

    /// <inheritdoc />
    public object Value
    {
        get => _value;
        set
        {
            object oldValue = _value;
            _value = value;
            Callbacks?.Invoke(oldValue, _value);
            _configFile.Save();
        }
    }

    /// <inheritdoc />
    public object Default { get; }

    /// <inheritdoc />
    public Type ValueType { get; }

    /// <inheritdoc />
    public T Get<T>() where T : class
    {
        if (!typeof(T).IsAssignableFrom(ValueType))
        {
            throw new InvalidCastException($"Cannot cast {ValueType} to {typeof(T)}");
        }

        return Value as T;
    }

    /// <inheritdoc />
    public void Set<T>(T value)
    {
        if (!ValueType.IsAssignableFrom(typeof(T)))
        {
            throw new InvalidCastException($"Cannot cast {ValueType} to {typeof(T)}");
        }
        if (Constraint != null)
        {
            if (!Constraint.IsConstrained(value)) return;
        }
        Value = Convert.ChangeType(value, ValueType);
    }

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public IValueConstraint? Constraint { get; }

    /// <inheritdoc />
    public void RegisterCallback(Action<object, object>? valueChangedCallback)
    {
        Callbacks += valueChangedCallback;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Tags => _tags;

    /// <inheritdoc />
    public bool HasTag(string tag) => _tags.Contains(tag);

    // Tags are declared at registration, not persisted, so a re-bind unions them in without touching the file.
    internal void MergeTags(IEnumerable<string>? tags)
    {
        if (tags == null) return;
        foreach (var tag in tags) _tags.Add(tag);
    }
}