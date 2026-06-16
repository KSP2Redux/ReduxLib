using MoonSharp.Interpreter;

namespace ReduxLib.GameInterfaces;

/// <summary>
/// The game-injected Lua runtime handoff. The game implements this and assigns <see cref="Instance" /> at
/// startup, exposing the root MoonSharp environment so the mod host (SpaceWarp) can fork per-mod
/// environments and run bodies itself. The underlying <see cref="Script" /> is reachable via
/// <c>RootGlobals.OwnerScript</c>.
/// </summary>
public interface IScriptRuntime
{
    public static IScriptRuntime Instance;

    /// <summary>
    /// The root environment's globals table. Per-mod environments are forked as child tables whose
    /// metatable <c>__index</c> falls through to this.
    /// </summary>
    Table RootGlobals { get; }
}
