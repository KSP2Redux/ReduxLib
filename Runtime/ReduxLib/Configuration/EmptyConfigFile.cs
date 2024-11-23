using System;
using System.Collections.Generic;

namespace ReduxLib.Configuration;

public class EmptyConfigFile : IConfigFile
{
    public void Save()
    {
        throw new NotImplementedException();
    }

    public IConfigEntry this[string section, string key] => throw new KeyNotFoundException($"{section}, {key}");

    public IConfigEntry Bind<T>(string section, string key, T? defaultValue = default, string description = "",
        IValueConstraint? constraint = null)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<string> Sections => Array.Empty<string>();

    public IReadOnlyList<string> this[string section] => Array.Empty<string>();
}