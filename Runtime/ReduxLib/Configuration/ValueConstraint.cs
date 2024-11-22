namespace ReduxLib.Configuration;

/// <summary>
/// Base class for value constraints.
/// </summary>
/// <typeparam name="T">Type of the value.</typeparam>
public abstract class ValueConstraint<T> : IValueConstraint
{
    /// <summary>
    /// Returns true if the given value is valid for this constraint.
    /// </summary>
    /// <param name="o">Value to check.</param>
    /// <returns>True if the value is valid, false otherwise.</returns>
    public abstract bool IsValid(T o);

    public bool IsConstrained(object o)
    {
        if (o is T t)
        {
            return IsValid(t);
        }

        return false;
    }

    public abstract string ConstraintDescription { get; }
}