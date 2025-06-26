using System;

namespace DG.Common.Caching
{
    /// <summary>
    /// Defines a set of operations for a strongly-typed in-memory cache.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITypedCache<T>
    {
        /// <summary>
        /// Sets a cache entry with the given <paramref name="key"/> and <paramref name="value"/>, indicating expiration based on the given <paramref name="expirationPolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationPolicy"></param>
        void Set(string key, T value, ExpirationPolicy expirationPolicy);

        /// <summary>
        /// Gets the entry associated with this <paramref name="key"/> if present.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The entry associated with this key, otherwise returns the <see langword="default"/> value of <typeparamref name="T"/>.</returns>
        T Get(string key);

        /// <summary>
        /// Gets the entry associated with the given <paramref name="key"/> if it exists, or generates a new value using the the given <paramref name="factory"/> and saves that using to the cache using <paramref name="expirationPolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <param name="expirationPolicy"></param>
        /// <returns></returns>
        T GetOrCreate(string key, Func<T> factory, ExpirationPolicy expirationPolicy);

        /// <summary>
        /// Tries to get the entry associated with the given <paramref name="key"/>, and returns a value indicating if it was found.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(string key, out T value);

        /// <summary>
        /// Returns a value indicating if an entry with the given <paramref name="key"/> exists in this cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(string key);

        /// <summary>
        /// Removes the entry associated with the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
    }
}
