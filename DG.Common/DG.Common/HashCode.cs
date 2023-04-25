using System;
using System.Collections.Generic;

namespace DG.Common
{
    /// <summary>
    /// Provides an easy way to implement <see cref="object.GetHashCode()"/> for classes.
    /// </summary>
    public struct HashCode : IEquatable<HashCode>
    {
        private const int _emptyCollectionPrimeNumber = 19;

        private readonly int _value;

        private HashCode(int value)
        {
            _value = value;
        }

        /// <summary>
        /// Implicitly converts this <see cref="HashCode"/> to an <see cref="int"/>.
        /// </summary>
        /// <param name="hashCode"></param>
        public static implicit operator int(HashCode hashCode)
        {
            return hashCode._value;
        }

        /// <summary>
        /// Indicates whether these hashcodes are the same.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(HashCode left, HashCode right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether these hashcodes are not the same.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(HashCode left, HashCode right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Creates a new <see cref="HashCode"/> from the given <typeparamref name="T"/> item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static HashCode Of<T>(T item)
        {
            return new HashCode(GetHashCode(item));
        }

        /// <summary>
        /// Creates a new <see cref="HashCode"/> from the given <see cref="IEnumerable{T}"/> items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static HashCode OfEach<T>(IEnumerable<T> items)
        {
            return items == null ? new HashCode(0) : new HashCode(GetHashCode(items, 0));
        }

        /// <summary>
        /// Adds the given <typeparamref name="T"/> item and returns a new <see cref="HashCode"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public HashCode And<T>(T item)
        {
            return new HashCode(CombineHashCodes(_value, GetHashCode(item)));
        }

        /// <summary>
        /// Adds the given <see cref="IEnumerable{T}"/> items and returns a new <see cref="HashCode"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public HashCode AndEach<T>(IEnumerable<T> items)
        {
            if (items == null)
            {
                return new HashCode(_value);
            }

            return new HashCode(GetHashCode(items, _value));
        }

        /// <inheritdoc/>
        public bool Equals(HashCode other)
        {
            return _value.Equals(other._value);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is HashCode))
            {
                return false;
            }

            return Equals((HashCode)obj);
        }

        private static int CombineHashCodes(int h1, int h2)
        {
            unchecked
            {
                //Bernstein's hash/djb2
                return ((h1 << 5) + h1) ^ h2;
            }
        }

        private static int GetHashCode<T>(T item)
        {
            return item?.GetHashCode() ?? 0;
        }

        private static int GetHashCode<T>(IEnumerable<T> items, int startHashCode)
        {
            var temp = startHashCode;

            var enumerator = items.GetEnumerator();
            if (enumerator.MoveNext())
            {
                temp = CombineHashCodes(temp, GetHashCode(enumerator.Current));

                while (enumerator.MoveNext())
                {
                    temp = CombineHashCodes(temp, GetHashCode(enumerator.Current));
                }
            }
            else
            {
                temp = CombineHashCodes(temp, _emptyCollectionPrimeNumber);
            }

            return temp;
        }
    }
}
