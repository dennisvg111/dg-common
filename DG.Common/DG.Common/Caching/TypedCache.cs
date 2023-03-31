using DG.Common.Locking;
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
        private readonly MemoryCache _cache;
        private readonly LockProvider _locks;
        private string cachePrefix => $"TypedCache<{typeof(T).FullName}>";

        /// <summary>
        /// The amount of minutes after a cache item has last been accessed after which it is removed from the cache.
        /// </summary>
        public int ExpirationMinutes { get; set; }
        private TimeSpan Expiration { get { return TimeSpan.FromMinutes(ExpirationMinutes); } }

        /// <summary>
        /// Initializes a new <see cref="TypedCache{T}"/>, with the given amount of minutes as initial value for <see cref="ExpirationMinutes"/>, using a default shared <see cref="MemoryCache"/>.
        /// </summary>
        /// <param name="expirationMinutes"></param>
        public TypedCache(int expirationMinutes) : this(expirationMinutes, MemoryCache.Default)
        {

        }

        /// <summary>
        /// Initializes a new <see cref="TypedCache{T}"/>, with the given amount of minutes as initial value for <see cref="ExpirationMinutes"/>, and the given <see cref="MemoryCache"/>.
        /// </summary>
        /// <param name="expirationMinutes"></param>
        /// <param name="cache"></param>
        public TypedCache(int expirationMinutes, MemoryCache cache)
        {
            _cache = cache;
            _locks = LockProvider.ByName(cachePrefix);
            ExpirationMinutes = expirationMinutes;
            if (ExpirationMinutes <= 0)
            {
                ExpirationMinutes = 30;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="TypedCache{T}"/>, with the given amount of minutes as initial value for <see cref="ExpirationMinutes"/>, and the given <see cref="MemoryCache"/>.
        /// </summary>
        /// <param name="expirationMinutes"></param>
        /// <param name="cacheName"></param>
        public TypedCache(int expirationMinutes, string cacheName) : this(expirationMinutes, new MemoryCache(cacheName))
        {
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
                _cache.Add(cachePrefix + key, savedItem, new CacheItemPolicy { SlidingExpiration = Expiration });
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
                return _cache[cachePrefix + key] as T;
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
                _cache.Remove(cachePrefix + key);
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
            if (value == null && !Contains(key))
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
                return _cache.Contains(cachePrefix + key);
            }
        }
    }
}
