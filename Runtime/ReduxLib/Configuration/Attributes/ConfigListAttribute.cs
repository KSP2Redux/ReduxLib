using System;
using System.Linq;
using System.Reflection;

namespace ReduxLib.Configuration.Attributes;

/// <summary>
/// Declares a list of acceptable values for a config property
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class ConfigListAttribute : Attribute
{
    /// <summary>
    /// The list of acceptable values
    /// </summary>
    public object[] AcceptableValues;

    /// <summary>
    /// Creates a new config list attribute with the given list of acceptable values
    /// </summary>
    /// <param name="acceptableValues">The list of acceptable values</param>
    public ConfigListAttribute(params object[] acceptableValues)
    {
        AcceptableValues = acceptableValues;
    }

    /// <summary>
    /// Converts this attribute to a list constraint of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>The list constraint</returns>
    public ListConstraint<T> ToListConstraintTyped<T>() where T : IEquatable<T>
    {
        return new ListConstraint<T>(AcceptableValues.Cast<T>().ToArray());
    }
    
    private static readonly MethodInfo ToListConstraintMethodInfo = typeof(ConfigListAttribute).GetMethod(nameof(ToListConstraintTyped), BindingFlags.Instance | BindingFlags.Public)!;

    /// <summary>
    /// Converts this attribute to a list constraint of t
    /// </summary>
    /// <param name="t">The type to convert to</param>
    /// <returns>The list constraint</returns>
    public IValueConstraint ToListConstraint(Type t)
    {
        return (IValueConstraint)ToListConstraintMethodInfo.MakeGenericMethod(t).Invoke(this, new object[] { });
    }
}