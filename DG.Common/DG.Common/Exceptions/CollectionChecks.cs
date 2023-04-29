using System;
using System.Collections.Generic;
using System.Linq;

namespace DG.Common.Exceptions
{
    /// <inheritdoc cref="ThrowIf"/>
    public class CollectionChecks<T>
    {
        private readonly IEnumerable<T> _input;
        private readonly string _paramName;

        /// <summary>
        /// The input for this check.
        /// </summary>
        public IEnumerable<T> Input => _input;

        /// <summary>
        /// The parameter name for this check, to be used to set <see cref="ArgumentException.ParamName"/>.
        /// </summary>
        public string ParamName => _paramName;

        internal CollectionChecks(IEnumerable<T> input, string paramName)
        {
            _input = input;
            _paramName = paramName;
        }

        /// <summary>
        /// Throws an exception if the collection contains no elements.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void IsEmpty(string message = null)
        {
            message = string.IsNullOrEmpty(message) ? "Collection cannot be empty." : message;
            if (!_input.Any())
            {
                throw new ArgumentException(_paramName, message);
            }
        }

        /// <summary>
        /// Throws an exception if the collection contains any elements matching the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Any(Func<T, bool> predicate, string message = null)
        {
            message = string.IsNullOrEmpty(message) ? "Collection cannot contain any element matching given predicate." : message;
            if (_input.Any(predicate))
            {
                throw new ArgumentException(_paramName, message);
            }
        }

        /// <summary>
        /// Throws an exception if the collection contains no elements matching the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void None(Func<T, bool> predicate, string message = null)
        {
            message = string.IsNullOrEmpty(message) ? "Collection must contain at least one element matching given predicate." : message;
            if (!_input.Any(predicate))
            {
                throw new ArgumentException(_paramName, message);
            }
        }

        /// <summary>
        /// Throws an exception if the size of the collection does not match the given <paramref name="count"/>.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void CountOtherThan(int count, string message = null)
        {
            message = string.IsNullOrEmpty(message) ? $"Collection must contain exactly {count} elements." : message;
            if (_input.Count() != count)
            {
                throw new ArgumentException(_paramName, message);
            }
        }

        /// <summary>
        /// Throws an exception if the size of the collection is less than the given <paramref name="count"/>.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void CountLessThan(int count, string message = null)
        {
            message = string.IsNullOrEmpty(message) ? $"Collection must contain {count} or more elements." : message;
            if (_input.Count() < count)
            {
                throw new ArgumentException(_paramName, message);
            }
        }

        /// <summary>
        /// Throws an exception if the size of the collection is more than the given <paramref name="count"/>.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void CountMoreThan(int count, string message = null)
        {
            message = string.IsNullOrEmpty(message) ? $"Collection must contain {count} or less elements." : message;
            if (_input.Count() > count)
            {
                throw new ArgumentException(_paramName, message);
            }
        }
    }
}
