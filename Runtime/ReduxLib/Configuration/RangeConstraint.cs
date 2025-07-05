using System;
using JetBrains.Annotations;

namespace ReduxLib.Configuration;

public class RangeConstraint<T> : ValueConstraint<T> where T : IComparable<T>
{
    /// <summary>
    /// The minimum value.
    /// </summary>
    [PublicAPI] public T Minimum;

    /// <summary>
    /// The maximum value.
    /// </summary>
    [PublicAPI] public T Maximum;

    public int Steps;
    public string Format;

    /// <summary>
    /// Creates a new range constraint.
    /// </summary>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    /// <param name="configSteps">The amount of steps on the slider in the config screen</param>
    /// <param name="configFormat">The format that the config slider prints the number</param>
    public RangeConstraint(T minimum, T maximum, int configSteps = 1024, string configFormat = "{0:F2}")
    {
        Minimum = minimum;
        Maximum = maximum;
        Steps = configSteps;
        Format = configFormat;
    }

    public override bool IsValid(T o) => Minimum.CompareTo(o) <= 0 && Maximum.CompareTo(o) >= 0;

    public override string ConstraintDescription => $"Accepts: {Minimum} - {Maximum}";
}