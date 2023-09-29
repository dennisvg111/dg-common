using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace DG.Common.InstanceManagement
{
    /// <summary>
    /// Provides functionality to create instances of a given type.
    /// <para></para>
    /// Note that these instances may be uninitialized if the type <typeparamref name="T"/> provides no default constructor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class UnsafeInstanceOf<T>
    {
        private static readonly Lazy<T> _instanceHolder = new Lazy<T>(() => New());

        /// <summary>
        /// Returns a singleton instance of the given type <typeparamref name="T"/>.
        /// </summary>
        public static T Shared => _instanceHolder.Value;

        /// <summary>
        /// Returns a new instance of the given type <typeparamref name="T"/>.
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
