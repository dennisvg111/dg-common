using DG.Common.GenericNumbers;
using System;

namespace DG.Common.Exceptions
{
    /// <inheritdoc cref="ThrowIf"/>
    public class NumberChecks<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        private readonly GenericNumber<T> _input;
        private readonly string _paramName;

        /// <summary>
        /// The input for this check.
        /// </summary>
        public T Input => _input.Value;

        /// <summary>
        /// The parameter name for this check, to be used to set <see cref="ArgumentException.ParamName"/>.
        /// </summary>
        public string ParamName => _paramName;

        internal NumberChecks(T input, string paramName)
        {
            _input = GenericNumber<T>.From(input);
            _paramName = paramName;
        }

        /// <summary>
        /// Throws an exception if the number is equal to zero.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void IsZero(string message = null)
        {
            if (_input == GenericNumber<T>.Zero)
            {
                message = string.IsNullOrEmpty(message) ? $"Number cannot be zero." : message;
                throw new ArgumentException(message, _paramName);
            }
        }

        /// <summary>
        /// Throws an exception if the number is less than zero.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void IsNegative(string message = null)
        {
            if (_input < GenericNumber<T>.Zero)
            {
                message = string.IsNullOrEmpty(message) ? $"Number cannot be less than zero." : message;
                throw new ArgumentException(message, _paramName);
            }
        }

        /// <summary>
        /// Throws an exception if the number is less than <paramref name="min"/> or more than <paramref name="max"/>.
        /// <para></para>
        /// Note: this means that the values for <paramref name="min"/> and <paramref name="max"/> are inclusive.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void IsNotBetweenInclusive(T min, T max, string message = null)
        {
            if (_input < min || _input > max)
            {
                message = string.IsNullOrEmpty(message) ? $"Number must be between {min} and {max}." : message;
                throw new ArgumentOutOfRangeException(_paramName, _input.Value, message);
            }
        }
    }
}
