﻿using System;
using System.Globalization;
using System.IO;

namespace DG.Common
{
    /// <summary>
    /// Provides functionality to format an amount of bytes.
    /// </summary>
    public struct ByteSize : IComparable<ByteSize>, IEquatable<ByteSize>
    {
        private const int _kibiByte = 1024;
        private const long _mebiByte = _kibiByte * _kibiByte;
        private const long _gibiByte = _mebiByte * _kibiByte;
        private const long _tebiByte = _gibiByte * _kibiByte;
        private const long _pebiByte = _tebiByte * _kibiByte;

        private const int _kiloByte = 1000;
        private const long _megaByte = _kiloByte * _kiloByte;
        private const long _gigaByte = _megaByte * _kiloByte;
        private const long _teraByte = _gigaByte * _kiloByte;
        private const long _petaByte = _teraByte * _kiloByte;

        private readonly long _byteCount;

        /// <summary>
        /// Returns the total amount of bytes this <see cref="ByteSize"/> represents.
        /// </summary>
        public long TotalBytes => _byteCount;

        private ByteSize(long byteCount)
        {
            _byteCount = byteCount;
        }

        private ByteSize(double units, UnitType unitType, long binaryUnit, long decimalUnit)
        {
            var bytesInUnit = (unitType == UnitType.Binary ? binaryUnit : decimalUnit);
            _byteCount = (long)Math.Round(units * bytesInUnit);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ByteSize"/> from the given amount of bytes.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static ByteSize FromBytes(long bytes)
        {
            return new ByteSize(bytes);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ByteSize"/> from the given byte array.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static ByteSize FromBytes(byte[] bytes)
        {
            return new ByteSize(bytes.LongLength);
        }

        /// <summary>
        /// <inheritdoc cref="FromBytes(long)"/>
        /// </summary>
        /// <param name="kB"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public static ByteSize FromKB(double kB, UnitType unitType = UnitType.Binary)
        {
            return new ByteSize(kB, unitType, _kibiByte, _kiloByte);
        }

        /// <summary>
        /// <inheritdoc cref="FromBytes(long)"/>
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public static ByteSize FromMB(double mb, UnitType unitType = UnitType.Binary)
        {
            return new ByteSize(mb, unitType, _mebiByte, _megaByte);
        }

        /// <summary>
        /// <inheritdoc cref="FromBytes(long)"/>
        /// </summary>
        /// <param name="gb"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public static ByteSize FromGB(double gb, UnitType unitType = UnitType.Binary)
        {
            return new ByteSize(gb, unitType, _gibiByte, _gigaByte);
        }

        /// <summary>
        /// <inheritdoc cref="FromBytes(long)"/>
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public static ByteSize FromTB(double tb, UnitType unitType = UnitType.Binary)
        {
            return new ByteSize(tb, unitType, _tebiByte, _teraByte);
        }

        /// <summary>
        /// <inheritdoc cref="FromBytes(long)"/>
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public static ByteSize FromPB(double pb, UnitType unitType = UnitType.Binary)
        {
            return new ByteSize(pb, unitType, _pebiByte, _petaByte);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ByteSize"/> from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ByteSize FromStream(Stream stream)
        {
            return new ByteSize(stream.Length);
        }

        /// <summary>
        /// Adds the given amount of bytes to the current amount, and returns a new instance of <see cref="ByteSize"/> for that amount.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public ByteSize AddBytes(long bytes)
        {
            return new ByteSize(_byteCount + bytes);
        }

        /// <summary>
        /// <inheritdoc cref="AddBytes(long)"/>
        /// </summary>
        /// <param name="kb"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public ByteSize AddKB(double kb, UnitType unitType = UnitType.Binary)
        {
            return this + FromKB(kb, unitType);
        }

        /// <summary>
        /// <inheritdoc cref="AddBytes(long)"/>
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public ByteSize AddMB(double mb, UnitType unitType = UnitType.Binary)
        {
            return this + FromMB(mb, unitType);
        }

        /// <summary>
        /// <inheritdoc cref="AddBytes(long)"/>
        /// </summary>
        /// <param name="gb"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public ByteSize AddGB(double gb, UnitType unitType = UnitType.Binary)
        {
            return this + FromGB(gb, unitType);
        }

        /// <summary>
        /// <inheritdoc cref="AddBytes(long)"/>
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public ByteSize AddTB(double tb, UnitType unitType = UnitType.Binary)
        {
            return this + FromTB(tb, unitType);
        }

        /// <summary>
        /// <inheritdoc cref="AddBytes(long)"/>
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public ByteSize AddPB(double pb, UnitType unitType = UnitType.Binary)
        {
            return this + FromPB(pb, unitType);
        }

        /// <summary>
        /// <inheritdoc cref="AddBytes(long)"/>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static ByteSize operator +(ByteSize left, ByteSize right)
        {
            return new ByteSize(left._byteCount + right._byteCount);
        }

        /// <summary>
        /// Returns a string representation of this <see cref="ByteSize"/>, using <see cref="FormattingStyle.Default"/> formatting, and <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(FormattingStyle.Default, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a string representation of this <see cref="ByteSize"/>, using <see cref="FormattingStyle.Default"/> formatting, and the given <see cref="IFormatProvider"/>.
        /// </summary>
        /// <returns></returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return ToString(FormattingStyle.Default, formatProvider);
        }

        /// <summary>
        /// <para>Returns a string representation of this <see cref="ByteSize"/>, using the given <see cref="FormattingStyle"/>, and optionally a <see cref="IFormatProvider"/>.</para>
        /// <para>Note that if <paramref name="formatProvider"/> is not given, <see cref="CultureInfo.InvariantCulture"/> will be used.</para>
        /// </summary>
        /// <returns></returns>
        public string ToString(FormattingStyle formatting, IFormatProvider formatProvider = null)
        {
            if (formatProvider == null)
            {
                formatProvider = CultureInfo.InvariantCulture;
            }
            int unit = (formatting == FormattingStyle.Default || formatting == FormattingStyle.IecBinary) ? _kibiByte : _kiloByte;
            string unitStr = "B";
            if (_byteCount < unit)
            {
                return string.Format(formatProvider, "{0} {1}", _byteCount, unitStr);
            }
            int exp = (int)(Math.Log(_byteCount) / Math.Log(unit));
            string sizePrefix = formatting == FormattingStyle.IecDecimal ? "kmgtpezy" : "KMGTPEZY";
            string unitSubfix = formatting == FormattingStyle.IecBinary ? "i" : string.Empty;
            double singleUnit = Math.Pow(unit, exp);
            var rounded = Math.Round(_byteCount / singleUnit, 2, MidpointRounding.AwayFromZero);
            return string.Format(formatProvider, "{0:##.##} {1}{2}{3}", rounded, sizePrefix[exp - 1], unitSubfix, unitStr);
        }

        /// <summary>
        /// Returns the amount of bytes this <see cref="ByteSize"/> represents.
        /// </summary>
        /// <returns></returns>
        public static implicit operator long(ByteSize byteSize)
        {
            return byteSize._byteCount;
        }

        #region Equality and comparison functions
        /// <inheritdoc/>
        public int CompareTo(ByteSize other)
        {
            return _byteCount.CompareTo(other._byteCount);
        }

        /// <inheritdoc/>
        public bool Equals(ByteSize other)
        {
            return _byteCount == other._byteCount;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is ByteSize))
            {
                return false;
            }
            return Equals((ByteSize)obj);
        }

        /// <summary>
        /// Returns a value indicating if the two amount of bytes are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ByteSize left, ByteSize right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating if the two amount of bytes are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ByteSize left, ByteSize right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a value indicating if one amount of bytes is larger than the other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(ByteSize left, ByteSize right)
        {
            return left._byteCount > right._byteCount;
        }

        /// <summary>
        /// Returns a value indicating if one amount of bytes is smaller than the other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(ByteSize left, ByteSize right)
        {
            return left._byteCount < right._byteCount;
        }

        /// <summary>
        /// Returns a value indicating if one amount of bytes is larger than or equal to the other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(ByteSize left, ByteSize right)
        {
            return left._byteCount >= right._byteCount;
        }

        /// <summary>
        /// Returns a value indicating if one amount of bytes is smaller than or equal to the other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(ByteSize left, ByteSize right)
        {
            return left._byteCount <= right._byteCount;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Of(_byteCount);
        }
        #endregion

        /// <summary>
        /// Represents the style used to format a <see cref="ByteSize"/> when creating a string representation.
        /// </summary>
        public enum FormattingStyle
        {
            /// <summary>
            /// Default Microsoft (Windows) formatting, 1024 bytes in a kilobyte, KB.
            /// </summary>
            Default,
            /// <summary>
            /// Binary formatting as according to the IEC standard, 1024 bytes in a kilobyte, KiB.
            /// </summary>
            IecBinary,
            /// <summary>
            /// Decimal formatting as according to the IEC standard, 1000 bytes in a kilobyte, kB.
            /// </summary>
            IecDecimal
        }

        /// <summary>
        /// Represents the kinds of units an amount of bytes can have.
        /// </summary>
        public enum UnitType
        {
            /// <summary>
            /// Binary units, works with powers of 1024.
            /// </summary>
            Binary,
            /// <summary>
            /// Decimal units, works with powers of 1000.
            /// </summary>
            Decimal
        }
    }
}
