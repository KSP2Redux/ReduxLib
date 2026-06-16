using System;
using System.Linq;
using System.Reflection;
using ReduxLib.Configuration.Attributes;

namespace ReduxLib.Configuration;

/// <summary>
/// Extensions to IConfigFile for binding to an object
/// </summary>
public static class ConfigExtensions
{
    private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static object? GetMemberValue(this MemberInfo m, object instance) => m switch
    {
        FieldInfo f => f.GetValue(instance),
        PropertyInfo p => p.GetValue(instance),
        _ => throw new InvalidOperationException($"Unsupported member: {m.MemberType}")
    };

    private static void SetMemberValue(this MemberInfo m, object instance, object? value)
    {
        switch (m)
        {
            case FieldInfo f: f.SetValue(instance, value); break;
            case PropertyInfo p: p.SetValue(instance, value); break;
            default: throw new InvalidOperationException($"Unsupported member: {m.MemberType}");
        }
    }

    static Type GetMemberType(this MemberInfo m) => m switch
    {
        FieldInfo f => f.FieldType,
        PropertyInfo p => p.PropertyType,
        _ => throw new InvalidOperationException($"Unsupported member: {m.MemberType}")
    };

    private static void GetRelevantAttributes(this MemberInfo m, out ConfigListAttribute? cla,
        out ConfigRangeAttribute? cra, out ConfigSectionAttribute? csa, out ConfigValueAttribute? cva)
    {
        cla = m.GetCustomAttribute<ConfigListAttribute>();
        cra = m.GetCustomAttribute<ConfigRangeAttribute>();
        cva = m.GetCustomAttribute<ConfigValueAttribute>();
        csa = m.GetCustomAttribute<ConfigSectionAttribute>();
    }

    private static bool IsConfigValueWrapper(this Type t, out Type inner)
    {
        for (var cur = t; cur != null && cur != typeof(object); cur = cur.BaseType)
        {
            if (!cur.IsGenericType || cur.GetGenericTypeDefinition() != typeof(ConfigValue<>)) continue;

            inner = cur.GetGenericArguments()[0];
            return true;
        }

        inner = null!;
        return false;
    }

    private static bool IsConfigDescriptionOrNull(this object? value)
    {
        if (value is null) return true;
        var t = value.GetType();
        return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ConfigDescription<>);
    }

    /// <summary>
    /// Bind the given object to the configuration file
    /// </summary>
    /// <param name="file">The current file</param>
    /// <param name="o">The object to bind</param>
    public static void Bind(this IConfigFile file, object o)
    {
        var type = o.GetType();

        var fields = type.GetFields(Flags).OrderBy(f => f.MetadataToken);
        var properties = type.GetProperties(Flags)
            .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
            .OrderBy(p => p.MetadataToken);
        var members = fields.Cast<MemberInfo>().Concat(properties);

        string? lastSectionName = null;
        string? lastSectionLoc = null;
        foreach (var member in members)
        {
            member.GetRelevantAttributes(out var cla, out var cra, out var csa, out var cva);
            if (csa != null)
            {
                lastSectionName = csa.Name;
                lastSectionLoc = csa.LocalizationKey;
            }

            if (cva == null) continue;
            var tags = member.GetCustomAttributes<ConfigMetadataAttribute>().Select(a => a.Tag).ToArray();
            if (lastSectionName == null)
            {
                throw new InvalidOperationException(
                    $"Cannot bind {type.Name}.{member.Name}: a [ConfigSection] must be declared before any [ConfigValue] member.");
            }
            var memberType = member.GetMemberType();
            var value = member.GetMemberValue(o);
            var section = file.GetOrCreateSection(lastSectionName, lastSectionLoc);
            if (memberType.IsConfigValueWrapper(out var wrappedType))
            {
                if (!value.IsConfigDescriptionOrNull())
                {
                    throw new Exception(
                        $"Cannot bind {member.Name} to config description as its assigned value must either be ConfigDescription<T> or null");
                }
                if (cla != null) RequireEquatable(wrappedType, member);
                if (cra != null) RequireComparable(wrappedType, member);
                member.SetMemberValue(o, _genericWrapping.MakeGenericMethod(wrappedType).Invoke(null, new object[]{section, cva, value, cla, cra, tags}));
            }
            else
            {
                IValueConstraint? constraint = null;
                if (cla != null)
                {
                    RequireEquatable(memberType, member);
                    constraint = cla.ToListConstraint(memberType);
                }
                else if (cra != null)
                {
                    RequireComparable(memberType, member);
                    constraint = cra.ToRangeConstraint(memberType);
                }

                var entry = (IConfigEntry)_bindPlainTyped.MakeGenericMethod(memberType)
                    .Invoke(null, new object[] { section, cva, value, constraint, tags })!;
                entry.RegisterCallback((_, n) => member.SetMemberValue(o, n));
            }
        }
    }

    private static readonly MethodInfo _bindPlainTyped = typeof(ConfigExtensions).GetMethod(
        nameof(BindPlainTyped), BindingFlags.Static | BindingFlags.NonPublic)!;

    private static IConfigEntry BindPlainTyped<T>(IConfigSection section, ConfigValueAttribute cva, T value, IValueConstraint? constraint, string[] tags)
        => section.BindEntry(cva.Name, value, cva.Description, constraint, cva.NameLocalizationKey, cva.DescriptionLocalizationKey, tags);

    private static void RequireEquatable(Type t, MemberInfo member)
    {
        var iface = typeof(IEquatable<>).MakeGenericType(t);
        if (!iface.IsAssignableFrom(t))
        {
            throw new InvalidOperationException(
                $"[ConfigList] on '{member.DeclaringType?.Name}.{member.Name}': type '{t.Name}' must implement IEquatable<{t.Name}>.");
        }
    }

    private static void RequireComparable(Type t, MemberInfo member)
    {
        var iface = typeof(IComparable<>).MakeGenericType(t);
        if (!iface.IsAssignableFrom(t))
        {
            throw new InvalidOperationException(
                $"[ConfigRange] on '{member.DeclaringType?.Name}.{member.Name}': type '{t.Name}' must implement IComparable<{t.Name}>.");
        }
    }

    private static MethodInfo _genericWrapping = typeof(ConfigExtensions).GetMethod(nameof(HandleGenericWrapping),
        BindingFlags.Static | BindingFlags.NonPublic)!;

    private static object HandleGenericWrapping<T>(IConfigSection section, ConfigValueAttribute cva, ConfigDescription<T>? description,
        ConfigListAttribute? cla, ConfigRangeAttribute? cra, string[] tags)
    {
        T? defValue = default;
        if (description != null)
        {
            defValue = description.DefaultValue ?? default;
        }
        var constraint = description?.Constraint ?? cla?.ToListConstraint(typeof(T)) ?? cra?.ToRangeConstraint(typeof(T));
        var entry = section.BindEntry(cva.Name, defValue, cva.Description, constraint, cva.NameLocalizationKey, cva.DescriptionLocalizationKey, tags);
        var result = new ConfigValue<T>(entry);
        if (description == null) return result;
        foreach (var cb in description.PreRegisteredCallbacks)
        {
            result.RegisterCallback(cb);
        }
        return result;
    }
}