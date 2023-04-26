using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace DG.Common.InstanceManagement
{
    /// <summary>
    /// Provides functionality to create uninitialized instances of a given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Uninitialized<T>
    {
        private static readonly T _instance;

        /// <summary>
        /// Returns an unitialized instance of the given type <typeparamref name="T"/>. Note that this always returns the same instance.
        /// </summary>
        public static T Instance => _instance;

        static Uninitialized()
        {
            _instance = New();
        }

        /// <summary>
        /// Returns a new unitialized instance of the given type <typeparamref name="T"/>.
        /// </summary>
        /// <returns></returns>
        public static T New()
        {
            Type t = typeof(T);
            if (HasDefaultConstructor(t))
            {
                var constructor = Expression.Lambda<Func<T>>(Expression.New(t)).Compile();
                return constructor();
            }

            return (T)FormatterServices.GetUninitializedObject(t);
        }

        private static bool HasDefaultConstructor(Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
