using System;
using System.IO;

namespace DG.Common.Exceptions
{
    /// <inheritdoc cref="ThrowIf"/>
    public class StreamChecks
    {
        private readonly Stream _input;
        private readonly string _paramName;

        /// <summary>
        /// The input for this check.
        /// </summary>
        public Stream Input => _input;

        /// <summary>
        /// The parameter name for this check, to be used to set <see cref="ArgumentException.ParamName"/>.
        /// </summary>
        public string ParamName => _paramName;

        internal StreamChecks(Stream input, string paramName)
        {
            _input = input;
            _paramName = paramName;
        }

        /// <summary>
        /// Throws an exception if the stream has a lenght of 0.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void IsEmpty(string message = null)
        {
            if (_input.Length == 0)
            {
                message = string.IsNullOrEmpty(message) ? "The stream cannot be empty." : message;
                throw new ArgumentException(_paramName, message);
            }
        }

        /// <summary>
        /// Throws an exception if the stream does not support seeking.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void CannotSeek(string message = null)
        {
            if (!_input.CanSeek)
            {
                message = string.IsNullOrEmpty(message) ? "This stream does not support seek operations." : message;
                throw new ArgumentException(_paramName, message);
            }
        }

        /// <summary>
        /// Throws an exception if the stream does not support reading.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void CannotRead(string message = null)
        {
            if (!_input.CanRead)
            {
                message = string.IsNullOrEmpty(message) ? "This stream does not support read operations." : message;
                throw new ArgumentException(_paramName, message);
            }
        }

        /// <summary>
        /// Throws an exception if the stream does not support seeking.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public void CannotWrite(string message = null)
        {
            if (!_input.CanWrite)
            {
                message = string.IsNullOrEmpty(message) ? "This stream does not support write operations." : message;
                throw new ArgumentException(_paramName, message);
            }
        }
    }
}
