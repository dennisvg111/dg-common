using DG.Common.Threading;
using System;
using System.Runtime.Caching;

namespace DG.Common.Caching
{
    /// <summary>
    /// Provides an easy way to create a strongly typed cache of values.
    /// <para></para>
    /// Note that this cache is shared with other instances of <see cref="TypedCache{T}"/> with the same type <typeparamref name="T"/>, unless a specific <see cref="MemoryCache"/> is given.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypedCache<T> where T : class
    {
        private readonly string _cachePrefix = $"TypedCache<{typeof(T).FullName}>";

        private readonly MemoryCache _cache;
        private readonly LockProvider _locks;
        private readonly ExpirationPolicy _expiration;

        /// <summary>
        /// Initializes a new <see cref="TypedCache{T}"/>, with the given <see cref="ExpirationPolicy"/>, using the default shared <see cref="MemoryCache"/>.
        /// </summary>
        /// <param name="expirationPolicy"></param>
        public TypedCache(ExpirationPolicy expirationPolicy) : this(expirationPolicy, MemoryCache.Default) { }

        /// <summary>
        /// Initializes a new <see cref="TypedCache{T}"/>, with the given <see cref="ExpirationPolicy"/>, and the given <see cref="MemoryCache"/>.
        /// </summary>
        /// <param name="expirationPolicy"></param>
        /// <param name="cache"></param>
        public TypedCache(ExpirationPolicy expirationPolicy, MemoryCache cache)
        {
            _cache = cache;
            _locks = LockProvider.ByName(_cachePrefix);
            _expiration = expirationPolicy;
        }

        /// <summary>
        /// Saves a new item to the cache with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="savedItem"></param>
        public void Save(string key, T savedItem)
        {
            lock (_locks.DefaultLock)
            {
                _cache.Add(_cachePrefix + key, savedItem, _expiration.GetPolicy());
            }
        }

        /// <summary>
        /// Retrieves the item with the specified <paramref name="key"/> from the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get(string key)
        {
            lock (_locks.DefaultLock)
            {
                return _cache[_cachePrefix + key] as T;
            }
        }

        /// <summary>
        /// Tries to retrieve an item with the specified <paramref name="key"/> from the cache. If an item with the specified <paramref name="key"/> is not present in the cache, creates it using the given <paramref name="creationFunction"/> and saves it to the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="creationFunction"></param>
        /// <returns></returns>
        public T GetOrCreate(string key, Func<T> creationFunction)
        {
            T value;
            if (TryGet(key, out value))
            {
                return value;
            }
            value = creationFunction();
            Save(key, value);
            return value;
        }

        /// <summary>
        /// Removes the item with the specified <paramref name="key"/> from the cache.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveFromCache(string key)
        {
            lock (_locks.DefaultLock)
            {
                _cache.Remove(_cachePrefix + key);
            }
        }

        /// <summary>
        /// Tries to retrieve an item with the specified <paramref name="key"/> from the cache, and returns a value indicating whether the item was retrieved successfully.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet(string key, out T value)
        {
            value = Get(key);
            if (!Contains(key))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns a value indicating whether an item with the specified <paramref name="key"/> exists in the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            lock (_locks.DefaultLock)
            {
                return _cache.Contains(_cachePrefix + key);
            }
        }
    }
}
