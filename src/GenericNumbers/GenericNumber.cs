using System;

namespace DG.Common.GenericNumbers
{
    /// <summary>
    /// Provides a way to use common number operators (such as ==, >, >=) when the type is not known.
    /// <para></para>
    /// To be considered a number, the type <typeparamref name="T"/> has to be a <see langword="struct"/>, and inherit from <see cref="IComparable"/>, <see cref="IComparable{T}"/>, <see cref="IConvertible"/>, <see cref="IEquatable{T}"/>, and <see cref="IFormattable"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct GenericNumber<T> : IEquatable<GenericNumber<T>> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        private readonly T _value;

        /// <summary>
        /// The underlying value of this number.
        /// </summary>
        public T Value => _value;

        private GenericNumber(T value)
        {
            _value = value;
        }

        /// <summary>
        /// Creates a new instance of <see cref="GenericNumber{T}"/> for the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static GenericNumber<T> From(T value)
        {
            return value;
        }

        /// <summary>
        /// An implicit operator converting the type <typeparamref name="T"/> to a <see cref="GenericNumber{T}"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator GenericNumber<T>(T value)
        {
            return new GenericNumber<T>(value);
        }

        /// <summary>
        /// Indicates whether the underlying <typeparamref name="T"/> values of two instances of <see cref="GenericNumber{T}"/> are considered equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(GenericNumber<T> left, GenericNumber<T> right)
        {
            return (left.Equals(right));
        }

        /// <summary>
        /// Indicates whether the underlying <typeparamref name="T"/> values of two instances of <see cref="GenericNumber{T}"/> are considered not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(GenericNumber<T> left, GenericNumber<T> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Indicates whether the underlying <typeparamref name="T"/> value of the left instance is greater than the value of the right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(GenericNumber<T> left, GenericNumber<T> right)
        {
            return left._value.CompareTo(right._value) > 0;
        }

        /// <summary>
        /// Indicates whether the underlying <typeparamref name="T"/> value of the left instance is less than the value of the right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(GenericNumber<T> left, GenericNumber<T> right)
        {
            return left._value.CompareTo(right._value) < 0;
        }

        /// <summary>
        /// Indicates whether the underlying <typeparamref name="T"/> value of the left instance is greater than or equal to the value of the right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(GenericNumber<T> left, GenericNumber<T> right)
        {
            return left._value.CompareTo(right._value) >= 0;
        }

        /// <summary>
        /// Indicates whether the underlying <typeparamref name="T"/> value of the left instance is less than or equal to the value of the right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(GenericNumber<T> left, GenericNumber<T> right)
        {
            return left._value.CompareTo(right._value) <= 0;
        }

        /// <summary>
        /// Returns value clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public T Clamp(T min, T max)
        {
            if (_value.CompareTo(min) < 0)
            {
                return min;
            }
            if (_value.CompareTo(max) > 0)
            {
                return max;
            }
            return _value;
        }

        /// <inheritdoc/>
        public bool Equals(GenericNumber<T> other)
        {
            return _value.Equals(other._value);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is GenericNumber<T> number)
            {
                return Equals(number);
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
        public static GenericNumber<T> Zero => _zero.Value;

        private static readonly Lazy<GenericNumber<T>> _zero = new Lazy<GenericNumber<T>>(() => new GenericNumber<T>(GenerateZero()));
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
