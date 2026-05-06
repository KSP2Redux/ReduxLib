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
    /// The number of slider steps in the settings menu
    /// </summary>
    public int Steps;
    /// <summary>
    /// The format string used by the settings menu slider
    /// </summary>
    public string Format;

    /// <summary>
    /// Creates a new config range attribute with the given min and max
    /// </summary>
    /// <param name="min">The min</param>
    /// <param name="max">The max</param>
    /// <param name="configSteps">The amount of steps on the slider in the settings menu</param>
    /// <param name="configFormat">The format used to display the value on the slider</param>
    public ConfigRangeAttribute(object min, object max, int configSteps = 1024, string configFormat = "{0:F2}")
    {
        Min = min;
        Max = max;
        Steps = configSteps;
        Format = configFormat;
    }

    /// <summary>
    /// Converts this attribute to a range constraint of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>The range constraint</returns>
    public RangeConstraint<T> ToRangeConstraintTyped<T>() where T : IComparable<T>
    {
        return new RangeConstraint<T>((T)Min, (T)Max, Steps, Format);
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