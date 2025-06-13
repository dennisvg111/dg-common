using DG.Common.Threading;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace DG.Common.Caching
{
    /// <summary>
    /// Provides an easy way to create a strongly typed cache of values.
    /// <para></para>
    /// Note that this cache is shared with other instances of <see cref="TypedCache{T}"/> with the same type <typeparamref name="T"/>, unless a specific <see cref="IMemoryCache"/> is given.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class TypedCache<T> where T : class
    {
        private static readonly Lazy<IMemoryCache> _sharedCacheProvider = new Lazy<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));

        private readonly string _cachePrefix = $"TypedCache<{typeof(T).FullName}>";

        private readonly IMemoryCache _cache;
        private readonly LockProvider _locks;
        private readonly ExpirationPolicy _expiration;

        /// <summary>
        /// Initializes a new <see cref="TypedCache{T}"/>, with the given <see cref="ExpirationPolicy"/>, using the default shared <see cref="MemoryCache"/>.
        /// </summary>
        /// <param name="expirationPolicy"></param>
        public TypedCache(ExpirationPolicy expirationPolicy) : this(expirationPolicy, _sharedCacheProvider.Value) { }

        /// <summary>
        /// Initializes a new <see cref="TypedCache{T}"/>, with the given <see cref="ExpirationPolicy"/>, and the given <see cref="MemoryCache"/>.
        /// </summary>
        /// <param name="expirationPolicy"></param>
        /// <param name="cache"></param>
        public TypedCache(ExpirationPolicy expirationPolicy, IMemoryCache cache)
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
                _cache.Set(_cachePrefix + key, savedItem, _expiration.GetCacheEntryOptions());
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
                return _cache.Get<T>(_cachePrefix + key);
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
            return _cache.GetOrCreate(_cachePrefix + key, (e) => creationFunction());
        }

        /// <summary>
        /// Removes the item with the specified <paramref name="key"/> from the cache.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveFromCache(string key)
        {
            _cache.Remove(_cachePrefix + key);
        }

        /// <summary>
        /// Tries to retrieve an item with the specified <paramref name="key"/> from the cache, and returns a value indicating whether the item was retrieved successfully.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet(string key, out T value)
        {
            return _cache.TryGetValue<T>(_cachePrefix + key, out value);
        }

        /// <summary>
        /// Returns a value indicating whether an item with the specified <paramref name="key"/> exists in the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return _cache.TryGetValue(_cachePrefix + key, out object _);
        }
    }
}
