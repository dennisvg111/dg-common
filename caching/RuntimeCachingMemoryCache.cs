#if NETFRAMEWORK
using System;
using System.Runtime.Caching;

namespace DG.Common.Caching.Memory
{
    internal class RuntimeCachingMemoryCache<T> : ITypedCache<T>
    {
        private readonly MemoryCache _cache;

        public RuntimeCachingMemoryCache(MemoryCache cache)
        {
            _cache = cache;
        }

        public void Set(string key, T value, ExpirationPolicy expirationPolicy)
        {
            var lazy = new Lazy<T>(() => value);
            _cache.Set(key, lazy, CreateCacheItemPolicy(expirationPolicy));
        }

        public T Get(string key)
        {
            var lazy = _cache.Get(key) as Lazy<T>;
            return lazy != null ? lazy.Value : default(T);
        }

        public bool TryGet(string key, out T value)
        {
            var cacheItem = _cache.GetCacheItem(key);
            if (cacheItem == null)
            {
                value = default(T);
                return false;
            }

            var lazy = cacheItem.Value as Lazy<T>;
            value = lazy != null ? lazy.Value : default(T);
            return true;
        }

        public T GetOrCreate(string key, Func<T> factory, ExpirationPolicy expirationPolicy)
        {
            var newLazy = new Lazy<T>(factory);
            var resultLazy = _cache.AddOrGetExisting(key, newLazy, CreateCacheItemPolicy(expirationPolicy)) as Lazy<T>;
            return resultLazy != null ? resultLazy.Value : newLazy.Value;
        }

        public bool Contains(string key)
        {
            return _cache.Contains(key);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        private CacheItemPolicy CreateCacheItemPolicy(ExpirationPolicy expirationPolicy)
        {
            return expirationPolicy.ConvertTo<CacheItemPolicy>((s, a) =>
            {
                var policy = new CacheItemPolicy();
                if (s.HasValue)
                {
                    policy.SlidingExpiration = s.Value;
                }
                if (a.HasValue)
                {
                    policy.AbsoluteExpiration = a.Value;
                }
                return policy;
            });
        }
    }
}
#endif