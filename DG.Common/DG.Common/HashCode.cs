using System;
using System.Collections.Generic;

namespace DG.Common
{
    public struct HashCode : IEquatable<HashCode>
    {
        private const int _emptyCollectionPrimeNumber = 19;

        private readonly int _value;

        private HashCode(int value)
        {
            _value = value;
        }

        public static implicit operator int(HashCode hashCode)
        {
            return hashCode._value;
        }

        public static bool operator ==(HashCode left, HashCode right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HashCode left, HashCode right)
        {
            return !(left == right);
        }

        public static HashCode Of<T>(T item)
        {
            return new HashCode(GetHashCode(item));
        }

        public static HashCode OfEach<T>(IEnumerable<T> items)
        {
            return items == null ? new HashCode(0) : new HashCode(GetHashCode(items, 0));
        }

        public HashCode And<T>(T item)
        {
            return new HashCode(CombineHashCodes(_value, GetHashCode(item)));
        }

        public HashCode AndEach<T>(IEnumerable<T> items)
        {
            if (items == null)
            {
                return new HashCode(_value);
            }

            return new HashCode(GetHashCode(items, _value));
        }

        public bool Equals(HashCode other)
        {
            return _value.Equals(other._value);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HashCode))
            {
                return false;
            }

            return Equals((HashCode)obj);
        }

        public override int GetHashCode()
        {
            return _value;
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
