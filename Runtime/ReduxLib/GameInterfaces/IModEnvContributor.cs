using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace ReduxLib.GameInterfaces;

/// <summary>
/// A subsystem that contributes its own globals into each per-mod environment the runtime creates.
/// </summary>
/// <remarks>
/// The mod host (SpaceWarp) owns env creation and the general mod-loading globals (ModId, Location, require).
/// Contributors add only what is specific to them. For example, PatchManager contributes <c>PM</c>/<c>J</c>.
/// </remarks>
public interface IModEnvContributor
{
    /// <summary>
    /// Contributes this subsystem's globals to a freshly forked mod environment.
    /// </summary>
    /// <remarks>
    /// The runtime has already contributed the general mod-loading globals (<c>ModId</c>, <c>Location</c>), so a
    /// contributor that needs them reads them back off <paramref name="globals" />.
    /// </remarks>
    /// <param name="globals">The mod environment's globals table.</param>
    void Contribute(Table globals);
}

/// <summary>
/// Registry of <see cref="IModEnvContributor" />s.
/// </summary>
/// <remarks>
/// Subsystems register here at startup. The runtime runs them for each mod environment it creates.
/// </remarks>
public static class ModRuntime
{
    /// <summary>
    /// The registered contributors run against every mod environment the runtime creates.
    /// </summary>
    public static readonly List<IModEnvContributor> Contributors = new();
}
