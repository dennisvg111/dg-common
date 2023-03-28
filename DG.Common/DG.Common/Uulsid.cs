using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace DG.Common
{
    /// <summary>
    /// Universally Unique Lexicographically Sortable Identifier.
    /// <para></para>
    /// The first bytes in this object indicate the creation time, to allow for lexicographical sorting. The other bytes are added for uniqueness.
    /// </summary>
    public class Uulsid : IComparable, IComparable<Uulsid>, IEquatable<Uulsid>
    {
        private static readonly object _lock = new object();

        private readonly Timestamp _timestampBytes;
        private readonly RandomBytes _randomBytes;

        /// <summary>
        /// Gets the Date part (UTC) of this <see cref="Uulsid"/>.
        /// </summary>
        /// <returns></returns>
        public DateTime GetAsDate()
        {
            return _timestampBytes.GetAsDate();
        }

        private Uulsid(Timestamp timestamp, RandomBytes randomness)
        {
            _timestampBytes = timestamp;
            _randomBytes = randomness;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Uulsid"/> structure.
        /// </summary>
        /// <returns>A new unique UULSID instance.</returns>
        public static Uulsid NewUulsid()
        {
            var timestamp = Timestamp.Now;
            return NewUulsidForTimestamp(timestamp);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Uulsid"/> structure, for the given date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>A new unique UULSID instance.</returns>
        public static Uulsid NewUulsidForDate(DateTime date)
        {
            var timestamp = Timestamp.FromDate(date);
            return NewUulsidForTimestamp(timestamp);
        }

        private static Uulsid NewUulsidForTimestamp(Timestamp timestamp)
        {
            lock (_lock)
            {
                var randomBytes = RandomBytes.ForTimestamp(timestamp);
                return new Uulsid(timestamp, randomBytes);
            }
        }

        /// <summary>
        /// Creates a string representation of this UULSID that can be parsed using either the <see cref="Parse(string)"/> or <see cref="TryParse(string, out Uulsid)"/> methods.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var base32 = Base32.From(_timestampBytes, _randomBytes);
            return base32.ToString();
        }

        /// <summary>
        /// Converts the string representation of an UULSID to an instance of <see cref="Uulsid"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static Uulsid Parse(string input)
        {
            Uulsid result;
            if (!TryParse(input, out result))
            {
                throw new FormatException("Input string was not in a correct UULSID format.");
            }
            return result;
        }

        /// <summary>
        /// Converts the string representation of an UULSID to an instance of <see cref="Uulsid"/>. A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="uulsid"></param>
        /// <returns></returns>
        public static bool TryParse(string input, out Uulsid uulsid)
        {
            uulsid = default(Uulsid);
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            if (!Base32.TryParse(input, out Base32 base32))
            {
                return false;
            }
            uulsid = base32.ToUulsid();
            return true;
        }

        /// <inheritdoc/>
        public bool Equals(Uulsid other)
        {
            return _timestampBytes.Equals(other._timestampBytes)
                && _randomBytes.Equals(other._randomBytes);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is Uulsid))
            {
                return false;
            }
            return Equals((Uulsid)obj);
        }

        /// <summary>
        /// Returns a value indicating if the two Uulsids are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Uulsid left, Uulsid right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating if the two Uulsids are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Uulsid left, Uulsid right)
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        public int CompareTo(Uulsid other)
        {
            var result = _timestampBytes.CompareTo(other._timestampBytes);
            if (result != 0)
            {
                return result;
            }
            return _randomBytes.CompareTo(other._randomBytes);
        }

        /// <inheritdoc/>
        public int CompareTo(object obj)
        {
            if (!(obj is Uulsid))
            {
                throw new ArgumentException($"Expected type {typeof(Uulsid).FullName}, received {obj.GetType().FullName}", nameof(obj));
            }
            return CompareTo((Uulsid)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Of(_timestampBytes)
                .And(_randomBytes);
        }

        private sealed class Timestamp : IEquatable<Timestamp>, IComparable<Timestamp>
        {
            public const int ByteCount = 6;
            public const int Base32Length = 10;

            private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            private const long _mask0 = 280375465082880;
            private const long _mask1 = 1095216660480;
            private const long _mask2 = 4278190080;
            private const long _mask3 = 16711680;
            private const long _mask4 = 65280;
            private const long _mask5 = 255;

            private readonly byte _byte0;
            private readonly byte _byte1;
            private readonly byte _byte2;
            private readonly byte _byte3;
            private readonly byte _byte4;
            private readonly byte _byte5;

            public Timestamp(long timeStamp)
            {
                _byte0 = (byte)((timeStamp & _mask0) >> 40);
                _byte1 = (byte)((timeStamp & _mask1) >> 32);
                _byte2 = (byte)((timeStamp & _mask2) >> 24);
                _byte3 = (byte)((timeStamp & _mask3) >> 16);
                _byte4 = (byte)((timeStamp & _mask4) >> 8);
                _byte5 = (byte)((timeStamp & _mask5) >> 0);
            }

            public Timestamp(byte[] timestamp)
            {
                if (timestamp.Length != ByteCount)
                {
                    throw new ArgumentException($"Expected a length of {ByteCount}, received {timestamp.Length}", nameof(timestamp));
                }

                _byte0 = timestamp[0];
                _byte1 = timestamp[1];
                _byte2 = timestamp[2];
                _byte3 = timestamp[3];
                _byte4 = timestamp[4];
                _byte5 = timestamp[5];
            }

            public static Timestamp Now
            {
                get
                {
                    DateTime now = DateTime.UtcNow;
                    long timestamp = (long)(now - _epoch).TotalMilliseconds;
                    return new Timestamp(timestamp);
                }
            }

            public static Timestamp FromDate(DateTime date)
            {
                date = date.ToUniversalTime();
                long timestamp = (long)(date - _epoch).TotalMilliseconds;
                return new Timestamp(timestamp);
            }

            public long MillisecondsSinceEpoch
            {
                get
                {
                    long t0 = _byte0;
                    long t1 = _byte1;
                    long t2 = _byte2;
                    long t3 = _byte3;
                    long t4 = _byte4;
                    long t5 = _byte5;

                    return (t0 << 40) | (t1 << 32) | (t2 << 24) | (t3 << 16) | (t4 << 8) | (t5);
                }
            }

            public DateTime GetAsDate()
            {
                return _epoch.AddMilliseconds(MillisecondsSinceEpoch);
            }

            public int[] ToBase32()
            {
                return new int[]
                {
                    (_byte0 & 224) >> 5,
                    _byte0 & 31,
                    (_byte1 & 248) >> 3,
                    ((_byte1 & 7) << 2) | ((_byte2 & 192) >> 6),
                    (_byte2 & 62) >> 1,
                    ((_byte2 & 1) << 4) | ((_byte3 & 240) >> 4),
                    ((_byte3 & 15) << 1) | ((_byte4 & 128) >> 7),
                    (_byte4 & 124) >> 2,
                    ((_byte4 & 3) << 3) | ((_byte5 & 224) >> 5),
                    _byte5 & 31
                };
            }

            public static Timestamp FromBase32(int[] base32)
            {
                var bytes = new byte[]
                {
                    (byte)(base32[0] << 5 | base32[1]),
                    (byte)(base32[2] << 3 | base32[3] >> 2),
                    (byte)(base32[3] << 6 | base32[4] << 1 | base32[5] >> 4),
                    (byte)(base32[5] << 4 | base32[6] >> 1),
                    (byte)(base32[6] << 7 | base32[7] << 2 | base32[8] >> 3),
                    (byte)(base32[8] << 5 | base32[9])
                };
                return new Timestamp(bytes);
            }

            public bool Equals(Timestamp other)
            {
                return _byte0 == other._byte0
                && _byte1 == other._byte1
                && _byte2 == other._byte2
                && _byte3 == other._byte3
                && _byte4 == other._byte4
                && _byte5 == other._byte5;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Timestamp))
                {
                    return false;
                }
                return Equals((Timestamp)obj);
            }

            public static bool operator ==(Timestamp left, Timestamp right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Timestamp left, Timestamp right)
            {
                return !(left == right);
            }

            public override int GetHashCode()
            {
                return HashCode.Of(_byte0)
                .And(_byte1)
                .And(_byte2)
                .And(_byte3)
                .And(_byte4)
                .And(_byte5);
            }

            public int CompareTo(Timestamp other)
            {
                return MillisecondsSinceEpoch.CompareTo(other.MillisecondsSinceEpoch);
            }
        }

        private sealed class RandomBytes : IEquatable<RandomBytes>, IComparable<RandomBytes>
        {
            public const int ByteCount = 10;
            public const int Base32Length = 16;

            private static readonly byte[] _lastUsedRandomBytes = new byte[ByteCount];
            private static Timestamp _lastUsedTimeStamp = new Timestamp(0);

            private readonly byte _byte0;
            private readonly byte _byte1;
            private readonly byte _byte2;
            private readonly byte _byte3;
            private readonly byte _byte4;
            private readonly byte _byte5;
            private readonly byte _byte6;
            private readonly byte _byte7;
            private readonly byte _byte8;
            private readonly byte _byte9;

            public RandomBytes(byte[] bytes)
            {
                if (bytes.Length != ByteCount)
                {
                    throw new ArgumentException($"Expected a length of {ByteCount}, received {bytes.Length}", nameof(bytes));
                }
                _byte0 = bytes[0];
                _byte1 = bytes[1];
                _byte2 = bytes[2];
                _byte3 = bytes[3];
                _byte4 = bytes[4];
                _byte5 = bytes[5];
                _byte6 = bytes[6];
                _byte7 = bytes[7];
                _byte8 = bytes[8];
                _byte9 = bytes[9];
            }

            public static RandomBytes ForTimestamp(Timestamp timestamp)
            {
                bool isTimestampSameAsLast = timestamp == _lastUsedTimeStamp;
                if (!isTimestampSameAsLast)
                {
                    _lastUsedTimeStamp = timestamp;
                }
                if (isTimestampSameAsLast)
                {
                    IncrementByteArray(_lastUsedRandomBytes);
                    return new RandomBytes(_lastUsedRandomBytes);
                }
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(_lastUsedRandomBytes);
                }
                return new RandomBytes(_lastUsedRandomBytes);
            }

            private static void IncrementByteArray(byte[] bytes)
            {
                for (int index = bytes.Length - 1; index >= 0; index--)
                {
                    if (bytes[index] < byte.MaxValue)
                    {
                        ++bytes[index];
                        return;
                    }
                    bytes[index] = 0;
                }
            }

            public int[] ToBase32()
            {
                return new int[]
                {
                    (_byte0 & 248) >> 3,
                    ((_byte0 & 7) << 2) | ((_byte1 & 192) >> 6),
                    (_byte1 & 62) >> 1,
                    ((_byte1 & 1) << 4) | ((_byte2 & 240) >> 4),
                    ((_byte2 & 15) << 1) | ((_byte3 & 128) >> 7),
                    (_byte3 & 124) >> 2,
                    ((_byte3 & 3) << 3) | ((_byte4 & 224) >> 5),
                    _byte4 & 31,
                    (_byte5 & 248) >> 3,
                    ((_byte5 & 7) << 2) | ((_byte6 & 192) >> 6),
                    (_byte6 & 62) >> 1,
                    ((_byte6 & 1) << 4) | ((_byte7 & 240) >> 4),
                    ((_byte7 & 15) << 1) | ((_byte8 & 128) >> 7),
                    (_byte8 & 124) >> 2,
                    ((_byte8 & 3) << 3) | ((_byte9 & 224) >> 5),
                    _byte9 & 31
                };
            }

            public static RandomBytes FromBase32(int[] base32)
            {
                var bytes = new byte[]
                {
                    (byte)(base32[0] << 3 | base32[1] >> 2),
                    (byte)(base32[1] << 6 | base32[2] << 1 | base32[3] >> 4),
                    (byte)(base32[3] << 4 | base32[4] >> 1),
                    (byte)(base32[4] << 7 | base32[5] << 2 | base32[6] >> 3),
                    (byte)(base32[6] << 5 | base32[7]),
                    (byte)(base32[8] << 3 | base32[9] >> 2),
                    (byte)(base32[9] << 6 | base32[10] << 1 | base32[11] >> 4),
                    (byte)(base32[11] << 4 | base32[12] >> 1),
                    (byte)(base32[12] << 7 | base32[13] << 2 | base32[14] >> 3),
                    (byte)(base32[14] << 5 | base32[15]),
                };
                return new RandomBytes(bytes);
            }

            public int CompareTo(RandomBytes other)
            {
                var thisBase32 = this.ToBase32();
                var otherBase32 = other.ToBase32();
                for (int i = 0; i < thisBase32.Length; i++)
                {
                    if (thisBase32[i] == otherBase32[i])
                    {
                        continue;
                    }
                    return thisBase32[i] < otherBase32[i] ? -1 : 1;
                }
                return 0;
            }

            public bool Equals(RandomBytes other)
            {
                return _byte0 == other._byte0
                && _byte1 == other._byte1
                && _byte2 == other._byte2
                && _byte3 == other._byte3
                && _byte4 == other._byte4
                && _byte5 == other._byte5
                && _byte6 == other._byte6
                && _byte7 == other._byte7
                && _byte8 == other._byte8
                && _byte9 == other._byte9;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is RandomBytes))
                {
                    return false;
                }
                return Equals((RandomBytes)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Of(_byte0)
                .And(_byte1)
                .And(_byte2)
                .And(_byte3)
                .And(_byte4)
                .And(_byte5)
                .And(_byte6)
                .And(_byte7)
                .And(_byte8)
                .And(_byte9);
            }
        }

        private static class CrockfordBase32Characters
        {
            public const char DelimiterCharacter = '-';

            /// <summary>
            /// Crockford's Base32. This alphabet excludes the letters I, L, O, and U to avoid confusion.
            /// </summary>
            private static readonly char[] _base32Characters = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };

            private static readonly Dictionary<char, int> _characterIndex;

            static CrockfordBase32Characters()
            {
                _characterIndex = Enumerable.Range(0, _base32Characters.Length).ToDictionary(i => _base32Characters[i]);
            }

            public static char GetCharacter(int base32Value)
            {
                return _base32Characters[base32Value];
            }

            public static bool TryGetBase32Value(char character, out int base32Value)
            {
                return _characterIndex.TryGetValue(character, out base32Value);
            }
        }

        private sealed class Base32
        {
            private readonly int[] _base32;
            public const int _length = Timestamp.Base32Length + RandomBytes.Base32Length;

            private Base32()
            {
                _base32 = new int[_length];
            }

            private Base32(int[] timestampBase32, int[] randomBase32) : this()
            {
                Array.Copy(timestampBase32, 0, _base32, 0, Timestamp.Base32Length);
                Array.Copy(randomBase32, 0, _base32, Timestamp.Base32Length, RandomBytes.Base32Length);
            }

            public int this[int index]
            {
                get { return _base32[index]; }
                set { _base32[index] = value; }
            }

            public static Base32 From(Timestamp timestamp, RandomBytes randomBytes)
            {
                return new Base32(timestamp.ToBase32(), randomBytes.ToBase32());
            }

            public static bool TryParse(string input, out Base32 base32)
            {
                base32 = new Base32();
                int i = 0;
                int inputOffset = 0;
                do
                {
                    char c = char.ToLowerInvariant(input[i + inputOffset]);
                    if (c == CrockfordBase32Characters.DelimiterCharacter)
                    {
                        inputOffset++;
                        continue;
                    }
                    if (!CrockfordBase32Characters.TryGetBase32Value(c, out int base32Value))
                    {
                        return false;
                    }
                    if (i >= _length)
                    {
                        return false;
                    }
                    base32[i] = base32Value;
                    i++;
                } while (i + inputOffset < input.Length);
                return i == _length;
            }

            public Uulsid ToUulsid()
            {
                var timestampBase32 = new int[Timestamp.Base32Length];
                Array.Copy(_base32, 0, timestampBase32, 0, Timestamp.Base32Length);
                var timestamp = Timestamp.FromBase32(timestampBase32);

                var randomBytesBase32 = new int[RandomBytes.Base32Length];
                Array.Copy(_base32, Timestamp.Base32Length, randomBytesBase32, 0, RandomBytes.Base32Length);
                var randomBytes = RandomBytes.FromBase32(randomBytesBase32);

                return new Uulsid(timestamp, randomBytes);
            }

            public override string ToString()
            {
                var stringChars = new char[_length + 1];
                for (int i = 0; i < Timestamp.Base32Length; i++)
                {
                    stringChars[i] = CrockfordBase32Characters.GetCharacter(_base32[i]);
                }
                stringChars[Timestamp.Base32Length] = CrockfordBase32Characters.DelimiterCharacter;
                for (int i = 0; i < RandomBytes.Base32Length; i++)
                {
                    stringChars[i + Timestamp.Base32Length + 1] = CrockfordBase32Characters.GetCharacter(_base32[i + Timestamp.Base32Length]);
                }
                return new string(stringChars);
            }
        }
    }
}
