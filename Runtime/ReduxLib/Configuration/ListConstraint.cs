using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using System.Linq;

namespace ReduxLib.Configuration;

public class ListConstraint<T> :  ValueConstraint<T> where T : IEquatable<T>
{
    
    /// <summary>
    /// The list of acceptable values.
    /// </summary>
    [PublicAPI] public List<T> AcceptableValues;

    /// <summary>
    /// Creates a new list constraint.
    /// </summary>
    /// <param name="acceptableValues">The list of acceptable values.</param>
    public ListConstraint(IEnumerable<T> acceptableValues)
    {
        AcceptableValues = acceptableValues.ToList();
    }
    
    /// <summary>
    /// Creates a new list constraint.
    /// </summary>
    /// <param name="acceptableValues">The list of acceptable values.</param>
    public ListConstraint(params T[] acceptableValues)
    {
        AcceptableValues = acceptableValues.ToList();
    }

    public override bool IsValid(T o) => AcceptableValues.Any(x => x.Equals(o));

    public override string ConstraintDescription
    {
        get
        {
            var sb = new StringBuilder();
            sb.Append("Accepts any of: ");
            var isFirst = true;
            foreach (var constraint in AcceptableValues)
            {
                if (!isFirst)
                {
                    sb.Append(", ");
                }
                isFirst = false;
                sb.Append(constraint);
            }

            return sb.ToString();
        }
    }
}