using System;
using System.Collections.Concurrent;

namespace DG.Common.Caching
{
    /// <summary>
    /// Provides methods to get or create an instance of <see cref="ITypedCache{T}"/>, using a given <see cref="ICacheFactory"/>.
    /// </summary>
    public class CacheProvider
    {
        private readonly ICacheFactory _cacheFactory;

        private readonly ConcurrentDictionary<Type, object> _sharedCaches = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> _namedCaches = new ConcurrentDictionary<Type, ConcurrentDictionary<string, object>>();

        /// <summary>
        /// Creates a new instance of <see cref="CacheProvider"/> with the specified <paramref name="cacheFactory"/>.
        /// </summary>
        /// <param name="cacheFactory"></param>
        public CacheProvider(ICacheFactory cacheFactory)
        {
            _cacheFactory = cacheFactory;
        }

        /// <summary>
        /// Get a <see cref="ITypedCache{T}"/> instance that is shared for this <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>An instance of <see cref="ITypedCache{T}"/> that is reused for this <typeparamref name="T"/>.</returns>
        public ITypedCache<T> Shared<T>()
        {
            return _sharedCaches.GetOrAdd(typeof(T), (_) => _cacheFactory.CreateNew<T>()) as ITypedCache<T>;
        }

        /// <summary>
        /// Get a <see cref="ITypedCache{T}"/> instance that is shared for this <paramref name="name"/> and <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns>An instance of <see cref="ITypedCache{T}"/> that is reused for this <paramref name="name"/>.</returns>
        public ITypedCache<T> Named<T>(string name)
        {
            var cacheCollection = _namedCaches.GetOrAdd(typeof(T), new ConcurrentDictionary<string, object>());
            return cacheCollection.GetOrAdd(name, (_) => _cacheFactory.CreateNew<T>()) as ITypedCache<T>;
        }

        /// <summary>
        /// Create a new <see cref="ITypedCache{T}"/> instance, using the default options.
        /// </summary>
        /// <returns>A new instance of <see cref="ITypedCache{T}"/>.</returns>
        public virtual ITypedCache<T> CreateNew<T>()
        {
            return _cacheFactory.CreateNew<T>();
        }
    }
}
