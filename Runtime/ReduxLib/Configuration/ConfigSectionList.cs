using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ReduxLib.Configuration;

/// <summary>
/// An ordered list of <see cref="IConfigSection" /> that is also indexable by section name.
/// </summary>
[PublicAPI]
public class ConfigSectionList : IReadOnlyList<IConfigSection>
{
    private readonly List<IConfigSection> _ordered = new();
    private readonly Dictionary<string, IConfigSection> _byName = new();

    /// <inheritdoc />
    public IConfigSection this[int index] => _ordered[index];

    /// <summary>
    /// Gets the section with the specified name.
    /// </summary>
    public IConfigSection this[string name] => _byName[name];

    /// <summary>
    /// Attempts to get a section by name.
    /// </summary>
    public bool TryGet(string name, out IConfigSection? section) => _byName.TryGetValue(name, out section);

    /// <summary>
    /// Whether a section with the given name exists.
    /// </summary>
    public bool Contains(string name) => _byName.ContainsKey(name);

    /// <inheritdoc />
    public int Count => _ordered.Count;

    /// <inheritdoc />
    public IEnumerator<IConfigSection> GetEnumerator() => _ordered.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal void Add(IConfigSection section)
    {
        if (_byName.ContainsKey(section.Name))
        {
            throw new ArgumentException($"Section '{section.Name}' already exists.", nameof(section));
        }
        _ordered.Add(section);
        _byName.Add(section.Name, section);
    }
}
