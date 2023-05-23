using System;
using System.Collections.Generic;
using System.IO;

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

        /// <summary>
        /// Provides methods to safeguard function parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static NumberChecks<T> Number<T>(T input, string paramName) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
        {
            return new NumberChecks<T>(input, paramName);
        }

        /// <summary>
        /// Provides methods to safeguard function parameters.
        /// </summary>
        public static StreamChecks Stream(Stream input, string paramName)
        {
            return new StreamChecks(input, paramName);
        }
    }
}
