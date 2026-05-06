using System;
using System.Reflection;

namespace ReduxLib.Configuration.Attributes;

/// <summary>
/// Declares a range of values that this config value accepts
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class ConfigRangeAttribute : Attribute
{
    /// <summary>
    /// The lower bound on the range of values
    /// </summary>
    public object Min;
    /// <summary>
    /// The upper bound on the range of values
    /// </summary>
    public object Max;

    /// <summary>
    /// Creates a new config range attribute with the given min and max
    /// </summary>
    /// <param name="min">The min</param>
    /// <param name="max">The max</param>
    public ConfigRangeAttribute(object min, object max)
    {
        Min = min;
        Max = max;
    }
    
    /// <summary>
    /// Converts this attribute to a range constraint of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>The range constraint</returns>
    public RangeConstraint<T> ToRangeConstraintTyped<T>() where T : IComparable<T>
    {
        return new RangeConstraint<T>((T)Min, (T)Max);
    }
    
    private static readonly MethodInfo ToRangeConstraintMethodInfo = typeof(ConfigRangeAttribute).GetMethod(nameof(ToRangeConstraintTyped), BindingFlags.Instance | BindingFlags.Public)!;

    /// <summary>
    /// Converts this attribute to a list constraint of t
    /// </summary>
    /// <param name="t">The type to convert to</param>
    /// <returns>The range constraint</returns>
    public IValueConstraint ToRangeConstraint(Type t)
    {
        return (IValueConstraint)ToRangeConstraintMethodInfo.MakeGenericMethod(t).Invoke(this, new object[] { });
    }
}