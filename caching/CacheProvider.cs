using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;

namespace DG.Common.Caching
{
    /// <summary>
    /// Provides methods to get a <see cref="IMemoryCache"/>, either shared, named, or globally unique.
    /// </summary>
    public static class CacheProvider
    {
        private static readonly ConcurrentDictionary<Type, IMemoryCache> _sharedCaches = new ConcurrentDictionary<Type, IMemoryCache>();
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, IMemoryCache>> _namedCaches = new ConcurrentDictionary<Type, ConcurrentDictionary<string, IMemoryCache>>();

        /// <summary>
        /// Get a <see cref="IMemoryCache"/> instance that is shared for this <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IMemoryCache Shared<T>()
        {
            return _sharedCaches.GetOrAdd(typeof(T), (_) => CreateNewCache());
        }

        /// <summary>
        /// Get a <see cref="IMemoryCache"/> instance that is shared for this <paramref name="name"/> and <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IMemoryCache Named<T>(string name)
        {
            var cacheCollection = _namedCaches.GetOrAdd(typeof(T), new ConcurrentDictionary<string, IMemoryCache>());
            return cacheCollection.GetOrAdd(name, (_) => CreateNewCache());
        }

        /// <summary>
        /// Create a new <see cref="IMemoryCache"/> instance, using the default options.
        /// </summary>
        /// <returns></returns>
        public static IMemoryCache CreateNewCache()
        {
            return new MemoryCache(new MemoryCacheOptions());
        }
    }
}
