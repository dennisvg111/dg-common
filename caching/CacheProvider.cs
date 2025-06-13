using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;

namespace DG.Common.Caching
{
    public static class CacheProvider
    {
        private static readonly ConcurrentDictionary<Type, IMemoryCache> _sharedCaches = new ConcurrentDictionary<Type, IMemoryCache>();
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, IMemoryCache>> _namedCaches = new ConcurrentDictionary<Type, ConcurrentDictionary<string, IMemoryCache>>();

        public static IMemoryCache Shared<T>()
        {
            return _sharedCaches.GetOrAdd(typeof(T), (_) => CreateNewCache());
        }
        public static IMemoryCache Named<T>(string name)
        {
            var cacheCollection = _namedCaches.GetOrAdd(typeof(T), new ConcurrentDictionary<string, IMemoryCache>());
            return cacheCollection.GetOrAdd(name, (_) => CreateNewCache());
        }

        public static IMemoryCache CreateNewCache()
        {
            return new MemoryCache(new MemoryCacheOptions());
        }
    }
}
