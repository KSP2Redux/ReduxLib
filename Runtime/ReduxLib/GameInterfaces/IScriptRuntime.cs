using MoonSharp.Interpreter;

namespace ReduxLib.GameInterfaces;

/// <summary>
/// The game-injected Lua runtime handoff.
/// </summary>
/// <remarks>
/// The game implements this and assigns <see cref="Instance" /> at startup, exposing the root MoonSharp
/// environment so the mod host (SpaceWarp) can fork per-mod environments and run bodies itself. The underlying
/// <see cref="Script" /> is reachable via <c>RootGlobals.OwnerScript</c>.
/// </remarks>
public interface IScriptRuntime
{
    /// <summary>
    /// The game-assigned runtime instance, set at startup.
    /// </summary>
    public static IScriptRuntime Instance;

    /// <summary>
    /// The root environment's globals table. Per-mod environments are forked as child tables whose
    /// metatable <c>__index</c> falls through to this.
    /// </summary>
    Table RootGlobals { get; }
}
