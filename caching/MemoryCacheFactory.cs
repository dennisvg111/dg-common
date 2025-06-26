#if NETFRAMEWORK
using System.Runtime.Caching;
#elif NETSTANDARD2_0 || NETCOREAPP || NET
using Microsoft.Extensions.Caching.Memory;
#endif

namespace DG.Common.Caching.Memory
{
#if NETFRAMEWORK
    /// <summary>
    /// An implementation of <see cref="ICacheFactory"/> that creates new instances of <see cref="ITypedCache{T}"/> using System.Runtime.Caching.
    /// </summary>
#elif NETSTANDARD2_0 || NETCOREAPP || NET
    /// <summary>
    /// An implementation of <see cref="ICacheFactory"/> that creates new instances of <see cref="ITypedCache{T}"/> using Microsoft.Extensions.Caching.Memory.
    /// </summary>
#endif
    public class MemoryCacheFactory : ICacheFactory
    {
        /// <inheritdoc />
        public virtual ITypedCache<T> CreateNew<T>()
        {
#if NETFRAMEWORK
            string cacheName = typeof(RuntimeCachingMemoryCache<T>).FullName + Uulsid.NewUulsid();
            return new RuntimeCachingMemoryCache<T>(new MemoryCache(cacheName));
#elif NETSTANDARD2_0 || NETCOREAPP || NET
            return new ExtensionsCachingMemoryCache<T>(new MemoryCache(new MemoryCacheOptions()));
#endif
        }
    }
}
