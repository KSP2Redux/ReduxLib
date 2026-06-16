using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace ReduxLib.GameInterfaces;

/// <summary>
/// A subsystem that seeds its own globals into each per-mod environment the runtime creates. The mod host
/// (SpaceWarp) owns env creation and the general mod-loading globals (ModId, Location, require) - contributors
/// add only what is specific to them (for example PatchManager seeds <c>PM</c>/<c>J</c>).
/// </summary>
public interface IModEnvContributor
{
    /// <summary>
    /// Seeds this contributor's globals into a freshly forked mod environment.
    /// </summary>
    /// <remarks>
    /// The runtime has already seeded the general mod-loading globals (<c>ModId</c>, <c>Location</c>), so a
    /// contributor that needs them reads them back off <paramref name="globals" />.
    /// </remarks>
    /// <param name="globals">The mod environment's globals table.</param>
    void Contribute(Table globals);
}

/// <summary>
/// Registry of <see cref="IModEnvContributor" />s. Subsystems register here at startup, the runtime runs them
/// for each mod environment it creates.
/// </summary>
public static class ModRuntime
{
    public static readonly List<IModEnvContributor> Contributors = new();
}
