using System;
using System.Collections.Generic;

namespace ReduxLib.Configuration;

/// <summary>
/// A fake config value that is used in the creation of defaults
/// </summary>
/// <typeparam name="T"></typeparam>
public class ConfigDescription<T> : ConfigValue<T>
{
    /// <summary>
    /// Create this fake config value, it should not be read before binding
    /// </summary>
    public ConfigDescription(T? def = default, ValueConstraint<T>? constraint = null) : base(null!)
    {
        DefaultValue = def;
        Constraint = constraint;
    }

    /// <summary>
    /// The set default value
    /// </summary>
    public T? DefaultValue;

    /// <summary>
    /// The given constraint on the value
    /// </summary>
    public ValueConstraint<T>? Constraint;

    /// <inheritdoc />
    public override IConfigEntry Entry => throw new Exception("Cannot access config value before binding.");

    /// <inheritdoc />
    public override T Value
    {
        get => throw new Exception("Cannot access config value before binding");
        set => throw new Exception("Cannot access config value before binding");
    }

    /// <summary>
    /// Pre-registered callbacks for this config value, just to make things easier
    /// </summary>
    public readonly List<Action<T, T>> PreRegisteredCallbacks = new List<Action<T, T>>();

    /// <inheritdoc />
    public override void RegisterCallback(Action<T, T> callback)
    {
        PreRegisteredCallbacks.Add(callback);
        return;
    }
}