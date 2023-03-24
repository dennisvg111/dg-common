using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace DG.Common
{
    /// <summary>
    /// Universally Unique Lexicographically Sortable Identifier
    /// </summary>
    public class Uulsid : IComparable, IComparable<Uulsid>, IEquatable<Uulsid>
    {

        private static long _lastUsedTimeStamp;
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

        public static Uulsid NewUulsid()
        {
            lock (_lock)
            {
                DateTime now = DateTime.UtcNow;
                long timestamp = (long)(now - Timestamp._epoch).TotalMilliseconds;
                return NewUulsidForTimestamp(timestamp);
            }
        }

        public static Uulsid NewUulsidForDate(DateTime date)
        {
            date = date.ToUniversalTime();
            long timestamp = (long)(date - Timestamp._epoch).TotalMilliseconds;
            return NewUulsidForTimestamp(timestamp);
        }

        private static Uulsid NewUulsidForTimestamp(long timestamp)
        {
            bool isTimestampSameAsLast = timestamp == _lastUsedTimeStamp;
            if (!isTimestampSameAsLast)
            {
                _lastUsedTimeStamp = timestamp;
            }
            RandomBytes randomBytes = RandomBytes.GetNewRandomBytes(isTimestampSameAsLast);
            return new Uulsid(new Timestamp(timestamp), randomBytes);
        }

        /// <summary>
        /// Renders this UULSID to a string that can be parsed using either the <see cref="Parse(string)"/> or <see cref="TryParse(string, out Uulsid)"/> methods.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var base32Timestamp = Base32.From(_timestampBytes);
            var base32Randomness = Base32.From(_randomBytes);
            var stringChars = new char[base32Timestamp.Length + base32Randomness.Length + 1];
            for (int i = 0; i < base32Timestamp.Length; i++)
            {
                stringChars[i] = CrockfordBase32Characters.GetCharacter(base32Timestamp[i]);
            }
            int offset = base32Timestamp.Length;
            stringChars[offset] = CrockfordBase32Characters.DelimiterCharacter;
            offset++;
            for (int i = 0; i < base32Randomness.Length; i++)
            {
                stringChars[i + offset] = CrockfordBase32Characters.GetCharacter(base32Randomness[i]);
            }
            return new string(stringChars);
        }

        public Uulsid Parse(string input)
        {
            Uulsid result;
            if (!TryParse(input, out result))
            {
                throw new FormatException("Input string was not in a correct UULSID format.");
            }
            return result;
        }

        public static bool TryParse(string input, out Uulsid uulsid)
        {
            uulsid = default(Uulsid);
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            if (!TryGetBase32FromString(input, out int[] base32))
            {
                return false;
            }
            uulsid = Base32.ConvertToUulsid(base32);
            return true;
        }

        private static bool TryGetBase32FromString(string input, out int[] base32)
        {
            base32 = new int[Base32.TimestampLength + Base32.RandomnessLength];
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
                if (i >= base32.Length)
                {
                    return false;
                }
                base32[i] = base32Value;
                i++;
            } while (i + inputOffset < input.Length);
            return true;
        }

        #region Equality and comparison functions

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
        #endregion

        private sealed class Timestamp : IEquatable<Timestamp>, IComparable<Timestamp>
        {
            public static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            public const int TimestampByteCount = 6;

            private const long _mask0 = 280375465082880;
            private const long _mask1 = 1095216660480;
            private const long _mask2 = 4278190080;
            private const long _mask3 = 16711680;
            private const long _mask4 = 65280;
            private const long _mask5 = 255;

            public Timestamp(long timeStamp)
            {
                TimeStamp_0 = (byte)((timeStamp & _mask0) >> 40);
                TimeStamp_1 = (byte)((timeStamp & _mask1) >> 32);
                TimeStamp_2 = (byte)((timeStamp & _mask2) >> 24);
                TimeStamp_3 = (byte)((timeStamp & _mask3) >> 16);
                TimeStamp_4 = (byte)((timeStamp & _mask4) >> 8);
                TimeStamp_5 = (byte)((timeStamp & _mask5) >> 0);
            }

            public Timestamp(byte[] timestamp)
            {
                if (timestamp.Length != TimestampByteCount)
                {
                    throw new ArgumentException($"Expected a length of {TimestampByteCount}, received {timestamp.Length}", nameof(timestamp));
                }

                TimeStamp_0 = timestamp[0];
                TimeStamp_1 = timestamp[1];
                TimeStamp_2 = timestamp[2];
                TimeStamp_3 = timestamp[3];
                TimeStamp_4 = timestamp[4];
                TimeStamp_5 = timestamp[5];
            }

            public byte TimeStamp_0 { get; }
            public byte TimeStamp_1 { get; }
            public byte TimeStamp_2 { get; }
            public byte TimeStamp_3 { get; }
            public byte TimeStamp_4 { get; }
            public byte TimeStamp_5 { get; }

            public long MillisecondsSinceEpoch
            {
                get
                {
                    long t0 = TimeStamp_0;
                    long t1 = TimeStamp_1;
                    long t2 = TimeStamp_2;
                    long t3 = TimeStamp_3;
                    long t4 = TimeStamp_4;
                    long t5 = TimeStamp_5;

                    return (t0 << 40) | (t1 << 32) | (t2 << 24) | (t3 << 16) | (t4 << 8) | (t5);
                }
            }

            public DateTime GetAsDate()
            {
                return _epoch.AddMilliseconds(MillisecondsSinceEpoch);
            }

            public bool Equals(Timestamp other)
            {
                return TimeStamp_0 == other.TimeStamp_0
                && TimeStamp_1 == other.TimeStamp_1
                && TimeStamp_2 == other.TimeStamp_2
                && TimeStamp_3 == other.TimeStamp_3
                && TimeStamp_4 == other.TimeStamp_4
                && TimeStamp_5 == other.TimeStamp_5;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Timestamp))
                {
                    return false;
                }
                return Equals((Timestamp)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Of(TimeStamp_0)
                .And(TimeStamp_1)
                .And(TimeStamp_2)
                .And(TimeStamp_3)
                .And(TimeStamp_4)
                .And(TimeStamp_5);
            }

            public int CompareTo(Timestamp other)
            {
                return MillisecondsSinceEpoch.CompareTo(other.MillisecondsSinceEpoch);
            }
        }

        private sealed class RandomBytes : IEquatable<RandomBytes>, IComparable<RandomBytes>
        {
            public const int RandomByteCount = 10;

            private static readonly byte[] _lastUsedRandomBytes = new byte[RandomByteCount];

            public RandomBytes(byte[] randomness)
            {
                if (randomness.Length != RandomByteCount)
                {
                    throw new ArgumentException($"Expected a length of {RandomByteCount}, received {randomness.Length}", nameof(randomness));
                }
                Randomness_0 = randomness[0];
                Randomness_1 = randomness[1];
                Randomness_2 = randomness[2];
                Randomness_3 = randomness[3];
                Randomness_4 = randomness[4];
                Randomness_5 = randomness[5];
                Randomness_6 = randomness[6];
                Randomness_7 = randomness[7];
                Randomness_8 = randomness[8];
                Randomness_9 = randomness[9];
            }

            public byte Randomness_0 { get; }
            public byte Randomness_1 { get; }
            public byte Randomness_2 { get; }
            public byte Randomness_3 { get; }
            public byte Randomness_4 { get; }
            public byte Randomness_5 { get; }
            public byte Randomness_6 { get; }
            public byte Randomness_7 { get; }
            public byte Randomness_8 { get; }
            public byte Randomness_9 { get; }

            /// <summary>
            /// Creates a new instance of <see cref="RandomBytes"/>, with new byte values.
            /// </summary>
            /// <param name="keepSeed">Indicates if the random bytes should continue with the same seed as last time.</param>
            /// <returns></returns>
            public static RandomBytes GetNewRandomBytes(bool keepSeed)
            {
                if (keepSeed)
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

            public int CompareTo(RandomBytes other)
            {
                var thisBase32 = Base32.From(this);
                var otherBase32 = Base32.From(other);
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
                return Randomness_0 == other.Randomness_0
                && Randomness_1 == other.Randomness_1
                && Randomness_2 == other.Randomness_2
                && Randomness_3 == other.Randomness_3
                && Randomness_4 == other.Randomness_4
                && Randomness_5 == other.Randomness_5
                && Randomness_6 == other.Randomness_6
                && Randomness_7 == other.Randomness_7
                && Randomness_8 == other.Randomness_8
                && Randomness_9 == other.Randomness_9;
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
                return HashCode.Of(Randomness_0)
                .And(Randomness_1)
                .And(Randomness_2)
                .And(Randomness_3)
                .And(Randomness_4)
                .And(Randomness_5)
                .And(Randomness_6)
                .And(Randomness_7)
                .And(Randomness_8)
                .And(Randomness_9);
            }
        }

        private static class CrockfordBase32Characters
        {
            public const char DelimiterCharacter = '-';

            /// <summary>
            /// Crockford's Base32. This alphabet excludes the letters I, L, O, and U to avoid confusion.
            /// </summary>
            public static readonly char[] _base32Characters = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };

            public static readonly Dictionary<char, int> _characterIndex;

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

        private static class Base32
        {
            public const int TimestampLength = 10;
            public const int RandomnessLength = 16;

            public static int[] From(Timestamp timestamp)
            {
                return GetBase32ForTimestamp(timestamp).ToArray();
            }

            public static int[] From(RandomBytes randomBytes)
            {
                return GetBase32ForRandomness(randomBytes).ToArray();
            }

            private static IEnumerable<int> GetBase32ForTimestamp(Timestamp timestamp)
            {
                yield return (timestamp.TimeStamp_0 & 224) >> 5;
                yield return timestamp.TimeStamp_0 & 31;
                yield return (timestamp.TimeStamp_1 & 248) >> 3;
                yield return ((timestamp.TimeStamp_1 & 7) << 2) | ((timestamp.TimeStamp_2 & 192) >> 6);
                yield return (timestamp.TimeStamp_2 & 62) >> 1;
                yield return ((timestamp.TimeStamp_2 & 1) << 4) | ((timestamp.TimeStamp_3 & 240) >> 4);
                yield return ((timestamp.TimeStamp_3 & 15) << 1) | ((timestamp.TimeStamp_4 & 128) >> 7);
                yield return (timestamp.TimeStamp_4 & 124) >> 2;
                yield return ((timestamp.TimeStamp_4 & 3) << 3) | ((timestamp.TimeStamp_5 & 224) >> 5);
                yield return timestamp.TimeStamp_5 & 31;
            }

            private static IEnumerable<int> GetBase32ForRandomness(RandomBytes randomBytes)
            {
                yield return (randomBytes.Randomness_0 & 248) >> 3;
                yield return ((randomBytes.Randomness_0 & 7) << 2) | ((randomBytes.Randomness_1 & 192) >> 6);
                yield return (randomBytes.Randomness_1 & 62) >> 1;
                yield return ((randomBytes.Randomness_1 & 1) << 4) | ((randomBytes.Randomness_2 & 240) >> 4);
                yield return ((randomBytes.Randomness_2 & 15) << 1) | ((randomBytes.Randomness_3 & 128) >> 7);
                yield return (randomBytes.Randomness_3 & 124) >> 2;
                yield return ((randomBytes.Randomness_3 & 3) << 3) | ((randomBytes.Randomness_4 & 224) >> 5);
                yield return randomBytes.Randomness_4 & 31;
                yield return (randomBytes.Randomness_5 & 248) >> 3;
                yield return ((randomBytes.Randomness_5 & 7) << 2) | ((randomBytes.Randomness_6 & 192) >> 6);
                yield return (randomBytes.Randomness_6 & 62) >> 1;
                yield return ((randomBytes.Randomness_6 & 1) << 4) | ((randomBytes.Randomness_7 & 240) >> 4);
                yield return ((randomBytes.Randomness_7 & 15) << 1) | ((randomBytes.Randomness_8 & 128) >> 7);
                yield return (randomBytes.Randomness_8 & 124) >> 2;
                yield return ((randomBytes.Randomness_8 & 3) << 3) | ((randomBytes.Randomness_9 & 224) >> 5);
                yield return randomBytes.Randomness_9 & 31;
            }

            public static Uulsid ConvertToUulsid(int[] base32)
            {
                var timestampBytes = new byte[]
                {
                    (byte)(base32[0] << 5 | base32[1]),
                    (byte)(base32[2] << 3 | base32[3] >> 2),
                    (byte)(base32[3] << 6 | base32[4] << 1 | base32[5] >> 4),
                    (byte)(base32[5] << 4 | base32[6] >> 1),
                    (byte)(base32[6] << 7 | base32[7] << 2 | base32[8] >> 3),
                    (byte)(base32[8] << 5 | base32[9])
                };
                var randomBytes = new byte[]
                {
                    (byte)(base32[10] << 3 | base32[11] >> 2),
                    (byte)(base32[11] << 6 | base32[12] << 1 | base32[13] >> 4),
                    (byte)(base32[13] << 4 | base32[14] >> 1),
                    (byte)(base32[14] << 7 | base32[15] << 2 | base32[16] >> 3),
                    (byte)(base32[16] << 5 | base32[17]),
                    (byte)(base32[18] << 3 | base32[19] >> 2),
                    (byte)(base32[19] << 6 | base32[20] << 1 | base32[21] >> 4),
                    (byte)(base32[21] << 4 | base32[22] >> 1),
                    (byte)(base32[22] << 7 | base32[23] << 2 | base32[24] >> 3),
                    (byte)(base32[24] << 5 | base32[25]),
                };
                return new Uulsid(new Timestamp(timestampBytes), new RandomBytes(randomBytes));
            }
        }
    }
}
