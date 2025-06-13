using Microsoft.Extensions.Caching.Memory;
using System;

namespace DG.Common.Caching
{
    /// <summary>
    /// Provides an easy way to create a strongly typed cache of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class TypedCache<T> where T : class
    {
        private readonly string _cachePrefix = $"TypedCache<{typeof(T).FullName}>";

        private readonly IMemoryCache _cache;
        private readonly ExpirationPolicy _expiration;

        /// <summary>
        /// Initializes a new <see cref="TypedCache{T}"/>, with the given <see cref="ExpirationPolicy"/>. Based on <paramref name="sharedOption"/> the cache will be shared with other instances for the given <typeparamref name="T"/>.
        /// </summary>
        /// <param name="expirationPolicy"></param>
        /// <param name="sharedOption"></param>
        public TypedCache(ExpirationPolicy expirationPolicy, CacheSharingOptions sharedOption = CacheSharingOptions.Unique)
            : this(expirationPolicy, sharedOption == CacheSharingOptions.Shared ? CacheProvider.Shared<T>() : CacheProvider.CreateNewCache()) { }

        /// <summary>
        /// Initializes a new <see cref="TypedCache{T}"/>, with the given <see cref="ExpirationPolicy"/>, sharing a cache with others that used the same <paramref name="cacheName"/> and <typeparamref name="T"/>.
        /// </summary>
        /// <param name="expirationPolicy"></param>
        /// <param name="cacheName"></param>
        public TypedCache(ExpirationPolicy expirationPolicy, string cacheName)
            : this(expirationPolicy, CacheProvider.Named<T>(cacheName)) { }

        /// <summary>
        /// Initializes a new <see cref="TypedCache{T}"/>, with the given <see cref="ExpirationPolicy"/>, and the given <see cref="IMemoryCache"/>.
        /// </summary>
        /// <param name="expirationPolicy"></param>
        /// <param name="cache"></param>
        public TypedCache(ExpirationPolicy expirationPolicy, IMemoryCache cache)
        {
            _cache = cache;
            _expiration = expirationPolicy;
        }

        private static MemoryCacheEntryOptions CreateOptions(TimeSpan? slidingExpiration, DateTimeOffset? absoluteExpiration)
        {
            return new MemoryCacheEntryOptions()
            {
                SlidingExpiration = slidingExpiration,
                AbsoluteExpiration = absoluteExpiration
            };
        }

        /// <summary>
        /// Saves a new item to the cache with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="savedItem"></param>
        public void Save(string key, T savedItem)
        {
            _cache.Set(_cachePrefix + key, savedItem, _expiration.ConvertTo((s, a) => CreateOptions(s, a)));
        }

        /// <summary>
        /// Retrieves the item with the specified <paramref name="key"/> from the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get(string key)
        {
            return _cache.Get<T>(_cachePrefix + key);
        }

        /// <summary>
        /// Tries to retrieve an item with the specified <paramref name="key"/> from the cache. If an item with the specified <paramref name="key"/> is not present in the cache, creates it using the given <paramref name="creationFunction"/> and saves it to the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="creationFunction"></param>
        /// <returns></returns>
        public T GetOrCreate(string key, Func<T> creationFunction)
        {
            return _cache.GetOrCreate(_cachePrefix + key, (e) => creationFunction(), _expiration.ConvertTo((s, a) => CreateOptions(s, a)));
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
