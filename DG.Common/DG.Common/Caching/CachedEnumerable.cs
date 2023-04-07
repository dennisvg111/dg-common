using System;
using System.Collections;
using System.Collections.Generic;

namespace DG.Common.Caching
{
    /// <summary>
    /// An implementation of <see cref="IEnumerable{T}"/> that caches ever item upon first enumeration.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CachedEnumerable<T> : IEnumerable<T>, IDisposable
    {
        private readonly IEnumerator<T> _enumerator;
        private readonly List<T> _cache;
        private readonly object _lock;
        private bool _cachingComplete;

        /// <summary>
        /// Initializes a new instance of <see cref="CachedEnumerable{T}"/> from the given <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="enumerable"></param>
        public CachedEnumerable(IEnumerable<T> enumerable) : this(enumerable.GetEnumerator()) { }

        /// <summary>
        /// Initializes a new instance of <see cref="CachedEnumerable{T}"/> from the given <see cref="IEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CachedEnumerable(IEnumerator<T> enumerator)
        {
            if (enumerator == null)
            {
                throw new ArgumentNullException(nameof(enumerator));
            }
            _enumerator = enumerator;
            _cache = new List<T>();
            _lock = new object();
            _cachingComplete = false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            int index = 0;
            while (true)
            {
                T current;
                lock (_lock)
                {
                    if (index >= _cache.Count && !TryMoveNext())
                    {
                        break;
                    }
                    current = _cache[index];
                    index++;
                }
                yield return current;
            }
        }

        private bool TryMoveNext()
        {
            if (_cachingComplete)
            {
                return false;
            }

            if (_enumerator != null && _enumerator.MoveNext())
            {
                _cache.Add(_enumerator.Current);
                return true;
            }
            _cachingComplete = true;

            //Release the enumerator, as it is no longer needed.
            Dispose();

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region IDisposable support

        private bool _isDisposed = false;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _enumerator.Dispose();
        }
        #endregion
    }
}
