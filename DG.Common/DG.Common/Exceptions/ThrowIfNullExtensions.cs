using System;
using System.Collections.Generic;
using System.Linq;

namespace DG.Common.Exceptions
{
    /// <summary>
    /// Provides extension methods for use with <see cref="Throws.If"/>.
    /// </summary>
    public static class ThrowIfNullExtensions
    {
        /// <summary>
        /// Throws an exception if the input is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="start"></param>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Null<T>(this Throws start, T input, string paramName, string message = null)
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
        /// <param name="start"></param>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void NullOrEmpty(this Throws start, string input, string paramName, string message = null)
        {
            Null(start, input, paramName, message);
            message = string.IsNullOrEmpty(message) ? "Parameter cannot be empty." : message;
            if (input == string.Empty)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// Throws an exception if the input is null, empty, or constists of only white-space characters.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void NullOrWhiteSpace(this Throws start, string input, string paramName, string message = null)
        {
            Null(start, input, paramName, message);
            message = string.IsNullOrEmpty(message) ? "Parameter cannot be empty or whitespace." : message;
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// Throws an exception if the input is null or contains no elements.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void NullOrEmpty<T>(this Throws start, IEnumerable<T> input, string paramName, string message = null)
        {
            Null(start, input, paramName, message);
            message = string.IsNullOrEmpty(message) ? "Parameter cannot be empty." : message;
            if (!input.Any())
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// Throws an exception if the input is null, or equal to the default value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void NullOrDefault<T>(this Throws start, T input, string paramName, string message = null)
        {
            Null(start, input, paramName, message);
            message = string.IsNullOrEmpty(message) ? "Parameter cannot be default value." : message;
            //better performance than object.Equals(value, default(T))
            if (EqualityComparer<T>.Default.Equals(input, default(T)))
            {
                throw new ArgumentException(message, paramName);
            }
        }
    }
}
