using DG.Common.GenericNumbers;
using System;

namespace DG.Common.Exceptions
{
    /// <inheritdoc cref="ThrowIf"/>
    public class NumberChecks<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        private readonly Operators<T> _input;
        private readonly string _paramName;

        /// <summary>
        /// The input for this check.
        /// </summary>
        public T Input => _input;

        /// <summary>
        /// The parameter name for this check, to be used to set <see cref="ArgumentException.ParamName"/>.
        /// </summary>
        public string ParamName => _paramName;

        internal NumberChecks(T input, string paramName)
        {
            _input = input;
            _paramName = paramName;
        }

        /// <summary>
        /// Throws an exception if the number is equal to zero.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void IsZero(string message = null)
        {
            if (_input == Operators<T>.Zero)
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Throws an exception if the number is less than zero.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void IsNegative()
        {
            if (_input < Operators<T>.Zero)
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Throws an exception if the number is less than <paramref name="min"/> or more than <paramref name="max"/>.
        /// <para></para>
        /// Note: this means that the values for <paramref name="min"/> and <paramref name="max"/> are inclusive.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <exception cref="ArgumentException"></exception>
        public void IsNotBetweenInclusive(T min, T max)
        {
            if (_input < min || _input > max)
            {
                throw new ArgumentException();
            }
        }
    }
}
