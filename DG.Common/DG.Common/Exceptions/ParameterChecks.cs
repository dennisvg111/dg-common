using System;
using System.Collections.Generic;
using System.Linq;

namespace DG.Common.Exceptions
{
    /// <inheritdoc cref="ThrowIf"/>
    public sealed class ParameterChecks
    {
        /// <summary>
        /// Parameter is invalid.
        /// </summary>
        private const string _defaultInvalidMessage = "Parameter is invalid.";

        internal ParameterChecks()
        {

        }

        /// <summary>
        /// Throws an exception if the input is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void IsNull<T>(T input, string paramName, string message = null)
        {
            message = string.IsNullOrEmpty(message) ? "Parameter cannot be null." : message;
            if (input == null)
            {
                throw new ArgumentNullException(paramName, message);
            }
        }

        /// <summary>
        /// Throws an exception if the input is null or empty.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void IsNullOrEmpty(string input, string paramName, string message = null)
        {
            IsNull(input, paramName, message);
            message = string.IsNullOrEmpty(message) ? "Parameter cannot be empty." : message;
            if (input == string.Empty)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// Throws an exception if the input is null, empty, or constists of only white-space characters.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void IsNullOrWhiteSpace(string input, string paramName, string message = null)
        {
            IsNull(input, paramName, message);
            message = string.IsNullOrEmpty(message) ? "Parameter cannot be empty or whitespace." : message;
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// Throws an exception if the input is null or contains no elements.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void IsNullOrEmpty<T>(IEnumerable<T> input, string paramName, string message = null)
        {
            IsNull(input, paramName, message);
            message = string.IsNullOrEmpty(message) ? "Parameter cannot be empty." : message;
            if (!input.Any())
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// Throws an exception if the input is null, or equal to the default value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void IsNullOrDefault<T>(T input, string paramName, string message = null)
        {
            IsNull(input, paramName, message);
            message = string.IsNullOrEmpty(message) ? "Parameter cannot be default value." : message;
            //better performance than object.Equals(value, default(T))
            if (EqualityComparer<T>.Default.Equals(input, default(T)))
            {
                throw new ArgumentException(message, paramName);
            }
        }


        /// <summary>
        /// Throws an exception if the input matches the given predicate.
        /// <para></para>
        /// Note it is recommended to use <see cref="Matches{T}(T, Func{T, bool}, string, string)"/> to specify a custom error message. Otherwise the message '<inheritdoc cref="_defaultInvalidMessage"/>' will be used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="predicate"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Matches<T>(T input, Func<T, bool> predicate, string paramName)
        {
            Matches(input, predicate, paramName, _defaultInvalidMessage);
        }

        /// <summary>
        /// Throws an exception if the input matches the given predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="predicate"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Matches<T>(T input, Func<T, bool> predicate, string paramName, string message)
        {
            message = string.IsNullOrEmpty(message) ? "Parameter is invalid." : message;
            if (predicate(input))
            {
                throw new ArgumentException(message, paramName);
            }
        }
    }
}
