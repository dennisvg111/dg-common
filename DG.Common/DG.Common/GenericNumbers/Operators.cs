using System;

namespace DG.Common.GenericNumbers
{
    public readonly struct Operators<T> : IEquatable<Operators<T>> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        private readonly T _value;

        public T Value => _value;

        public Operators(T value)
        {
            _value = value;
        }

        public static implicit operator Operators<T>(T value)
        {
            return new Operators<T>(value);
        }

        public static implicit operator T(Operators<T> value)
        {
            return value._value;
        }

        public static bool operator ==(Operators<T> left, Operators<T> right)
        {
            return (left.Equals(right));
        }

        public static bool operator !=(Operators<T> left, Operators<T> right)
        {
            return !(left == right);
        }

        public static bool operator >(Operators<T> left, Operators<T> right)
        {
            return left._value.CompareTo(right._value) > 0;
        }

        public static bool operator <(Operators<T> left, Operators<T> right)
        {
            return left._value.CompareTo(right._value) < 0;
        }

        public static bool operator >=(Operators<T> left, Operators<T> right)
        {
            return left._value.CompareTo(right._value) >= 0;
        }

        public static bool operator <=(Operators<T> left, Operators<T> right)
        {
            return left._value.CompareTo(right._value) <= 0;
        }

        /// <inheritdoc/>
        public bool Equals(Operators<T> other)
        {
            return _value.Equals(other._value);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if ((obj is Operators<T>))
            {
                return Equals((Operators<T>)obj);
            }
            return _value.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Of(_value);
        }

        /// <summary>
        /// Returns the value that represents zero for type <typeparamref name="T"/>.
        /// </summary>
        public static Operators<T> Zero => _zero.Value;

        private static readonly Lazy<Operators<T>> _zero = new Lazy<Operators<T>>(() => new Operators<T>(GenerateZero()));
        private static T GenerateZero()
        {
            try
            {
                return (T)Convert.ChangeType(0, typeof(T));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Returns a string representation of this number.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
