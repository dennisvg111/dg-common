using System;
using System.Collections.Generic;
using System.Linq;

namespace DG.Common.Exceptions
{
    /// <inheritdoc cref="ThrowIf"/>
    public class EnumerableChecks
    {
        internal EnumerableChecks() { }

        /// <summary>
        /// Throws an exception if the collection contains no elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void IsEmpty<T>(IEnumerable<T> input, string paramName, string message = null)
        {
            message = string.IsNullOrEmpty(message) ? "Collection cannot be empty." : message;
            if (!input.Any())
            {
                throw new ArgumentException(paramName, message);
            }
        }

        /// <summary>
        /// Throws an exception if the collection contains any elements matching the given predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="predicate"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void HasAny<T>(IEnumerable<T> input, Func<T, bool> predicate, string paramName, string message = null)
        {
            message = string.IsNullOrEmpty(message) ? "Collection cannot contain any element matching given predicate." : message;
            if (input.Any(predicate))
            {
                throw new ArgumentException(paramName, message);
            }
        }

        /// <summary>
        /// Throws an exception if the collection contains no elements matching the given predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="predicate"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void HasNo<T>(IEnumerable<T> input, Func<T, bool> predicate, string paramName, string message = null)
        {
            message = string.IsNullOrEmpty(message) ? "Collection must contain at least one element matching given predicate." : message;
            if (!input.Any(predicate))
            {
                throw new ArgumentException(paramName, message);
            }
        }
    }
}
