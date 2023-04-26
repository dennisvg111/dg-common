using System;

namespace DG.Common.InstanceManagement
{
    /// <summary>
    /// Provides an easy way to create a thread-safe singleton value.
    /// <para></para>
    /// Note that this singleton value is shared with other instances of <see cref="Singleton{T}"/> with the same type T.
    /// </summary>
    public static class Singleton
    {
        /// <summary>
        /// Gets a singleton instance of the given type <typeparamref name="T"/>, using the default constructor if it doesn't yet exist.
        /// <para></para>
        /// Note it is also possible to use <see cref="Singleton{T}.Instance"/> instead of this method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetInstance<T>() where T : new()
        {
            return GenericSingleton<T>.GetInstance(() => new T());
        }

        /// <summary>
        /// Gets a singleton instance of the given type <typeparamref name="T"/>, using the given <see cref="Func{T}"/> factory to create a new instance if it doesn't yet exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static T GetInstance<T>(Func<T> factory)
        {
            return GenericSingleton<T>.GetInstance(factory);
        }
    }

    /// <summary>
    /// Provides an easy way to create a thread-safe singleton value.
    /// <para></para>
    /// Note that this singleton value is shared with other instances of <see cref="Singleton{T}"/> with the same type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Singleton<T> where T : new()
    {
        /// <summary>
        /// Gets a singleton instance of the given type <typeparamref name="T"/> using the default constructor if it doesn't yet exist.
        /// </summary>
        /// <returns></returns>
        public static T Instance
        {
            get
            {
                return GenericSingleton<T>.GetInstance(() => new T());
            }
        }
    }


    internal sealed class GenericSingleton<T>
    {
        private static readonly object _lock = new object();
        private static T _instance;

        public static T GetInstance(Func<T> factory)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = factory();
                }
                return _instance;
            }
        }
    }
}
