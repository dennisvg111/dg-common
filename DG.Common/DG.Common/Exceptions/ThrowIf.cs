using System;
using System.Collections.Generic;

namespace DG.Common.Exceptions
{
    /// <summary>
    /// Provides methods that throw exceptions on specific conditions, to reduce boilerplate code.
    /// </summary>
    public static class ThrowIf
    {

        /// <summary>
        /// Provides methods to safeguard function parameters.
        /// </summary>
        public static ParameterChecks Parameter { get; } = new ParameterChecks();

        /// <summary>
        /// Provides methods to safeguard function parameters.
        /// </summary>
        public static CollectionChecks<T> Collection<T>(IEnumerable<T> input, string paramName)
        {
            return new CollectionChecks<T>(input, paramName);
        }

        public static NumberChecks<T> Number<T>(T input, string paramName) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            return new NumberChecks<T>(input, paramName);
        }
    }
}
