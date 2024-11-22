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
    
    /// <summary>
    /// Creates a new range constraint.
    /// </summary>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    public RangeConstraint(T minimum, T maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    public override bool IsValid(T o) => Minimum.CompareTo(o) <= 0 && Maximum.CompareTo(o) >= 0;

    public override string ConstraintDescription => $"Accepts: {Minimum} - {Maximum}";
}