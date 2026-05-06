using System;
using JetBrains.Annotations;

namespace ReduxLib.Configuration;

/// <summary>
/// A wrapper around <see cref="IConfigEntry"/> that provides type safety.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
[PublicAPI]
public class ConfigValue<T>
{
    private IConfigEntry _entry;

    /// <summary>
    /// The underlying <see cref="IConfigEntry"/>.
    /// </summary>
    public virtual IConfigEntry Entry => _entry;
    
    /// <summary>
    /// Creates a new <see cref="ConfigValue{T}"/> from an <see cref="IConfigEntry"/>.
    /// </summary>
    /// <param name="entry">The entry to wrap.</param>
    /// <exception cref="ArgumentException">
    /// If the type of <paramref name="entry"/> does not match <typeparamref name="T"/>.
    /// </exception>
    public ConfigValue(IConfigEntry entry)
    {
        _entry = entry;
        if (typeof(T) != entry.ValueType)
        {
            throw new ArgumentException(nameof(entry));
        }
    }
    
    /// <summary>
    /// The value of the entry.
    /// </summary>
    public virtual T Value
    {
        get => (T)_entry.Value;
        set => _entry.Value = value;
    }

    /// <summary>
    /// Registers a callback that will be invoked when the value changes.
    /// </summary>
    /// <param name="callback">The callback to invoke.</param>
    public virtual void RegisterCallback(Action<T, T> callback)
    {
        // Callbacks += callback;
        _entry.RegisterCallback(NewCallback);
        return;

        void NewCallback(object from, object to)
        {
            callback.Invoke((T)from, (T)to);
        }
    }
}