namespace ReduxLib.Configuration;

/// <summary>
/// Well-known config metadata tags defined by the config layer itself.
/// </summary>
/// <remarks>
/// Other systems define their own tags elsewhere. For example, PatchManager owns its cache-invalidation tag.
/// </remarks>
public static class ConfigTags
{
    /// <summary>
    /// Hides the entry from the settings menu.
    /// </summary>
    /// <remarks>
    /// The menu builder skips entries carrying this tag, so a mod can keep config a script reads without
    /// cluttering the UI.
    /// </remarks>
    public const string Hidden = "Hidden";
}
