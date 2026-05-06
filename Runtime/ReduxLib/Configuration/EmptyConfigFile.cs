using System;

namespace ReduxLib.Configuration;

public class EmptyConfigFile : IConfigFile
{
    public void Save() => throw new NotImplementedException();

    public ConfigSectionList Sections { get; } = new();

    public IConfigSection GetOrCreateSection(string name, string? localizationKey) => throw new NotImplementedException();
}
