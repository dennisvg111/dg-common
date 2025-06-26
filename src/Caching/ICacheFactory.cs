namespace DG.Common.Caching
{
    /// <summary>
    /// Defines a factory that creates unique instances of <see cref="ITypedCache{T}"/>, that do not share their cache.
    /// </summary>
    public interface ICacheFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="ITypedCache{T}"/> with the default options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ITypedCache<T> CreateNew<T>();
    }
}