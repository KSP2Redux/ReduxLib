namespace ReduxLib.Configuration;

/// <summary>
/// Well-known config metadata tags defined by the config layer itself. Other systems define their own tags
/// elsewhere (for example PatchManager owns its cache-invalidation tag).
/// </summary>
public static class ConfigTags
{
    /// <summary>
    /// Hides the entry from the settings menu. The menu builder skips entries carrying this tag, so a mod can
    /// keep config a script reads without cluttering the UI.
    /// </summary>
    public const string Hidden = "Hidden";
}
