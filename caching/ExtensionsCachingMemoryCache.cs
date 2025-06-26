#if NETSTANDARD2_0 || NETCOREAPP || NET
using Microsoft.Extensions.Caching.Memory;
using System;

namespace DG.Common.Caching.Memory
{
    internal class ExtensionsCachingMemoryCache<T> : ITypedCache<T>
    {
        private readonly IMemoryCache _cache;

        public ExtensionsCachingMemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Set(string key, T value, ExpirationPolicy expirationPolicy)
        {
            _cache.Set(key, value, ConvertToOptions(expirationPolicy));
        }

        public T Get(string key)
        {
            return _cache.Get<T>(key);
        }

        public bool TryGet(string key, out T value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public T GetOrCreate(string key, Func<T> factory, ExpirationPolicy expirationPolicy)
        {
            return _cache.GetOrCreate(key, e => factory(), ConvertToOptions(expirationPolicy));
        }

        public bool Contains(string key)
        {
            return _cache.TryGetValue(key, out _);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        private static MemoryCacheEntryOptions ConvertToOptions(ExpirationPolicy expirationPolicy)
        {
            return expirationPolicy.ConvertTo<MemoryCacheEntryOptions>((s, a) =>
            {
                return new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = s,
                    AbsoluteExpiration = a
                };
            });
        }
    }
}
#endif