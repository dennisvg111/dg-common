using System;
using System.Security.Cryptography;

namespace DG.Common
{
    /// <summary>
    /// Universally Unique Lexicographically Sortable Identifier
    /// </summary>
    public class Uulsid : IComparable, IComparable<Uulsid>, IEquatable<Uulsid>
    {
        /// <summary>
        /// Crockford's Base32. This alphabet excludes the letters I, L, O, and U to avoid confusion.
        /// </summary>
        private static readonly char[] _base32Characters = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        private const int _timestampBytes = 6;
        private const int _randomnessBytes = 10;
        private const int _dataLength = _timestampBytes + _randomnessBytes;

        private static long _lastUsedTimeStamp;
        private static readonly byte[] _lastUsedRandomness = new byte[_randomnessBytes];

        private static readonly object _lock = new object();

        public byte TimeStamp_0 { get; set; }
        public byte TimeStamp_1 { get; set; }
        public byte TimeStamp_2 { get; set; }
        public byte TimeStamp_3 { get; set; }
        public byte TimeStamp_4 { get; set; }
        public byte TimeStamp_5 { get; set; }

        public long TimeStamp
        {
            get
            {
                return new TimestampHelper(new byte[] { TimeStamp_0, TimeStamp_1, TimeStamp_2, TimeStamp_3, TimeStamp_4, TimeStamp_5 }).TimeStamp;
            }
        }

        public DateTime GetAsDate()
        {
            return _epoch.AddMilliseconds(TimeStamp);
        }

        public byte Randomness_0 { get; set; }
        public byte Randomness_1 { get; set; }
        public byte Randomness_2 { get; set; }
        public byte Randomness_3 { get; set; }
        public byte Randomness_4 { get; set; }
        public byte Randomness_5 { get; set; }
        public byte Randomness_6 { get; set; }
        public byte Randomness_7 { get; set; }
        public byte Randomness_8 { get; set; }
        public byte Randomness_9 { get; set; }

        private Uulsid(TimestampHelper timestamp, byte[] randomness)
        {
            if (randomness.Length != _randomnessBytes)
            {
                throw new ArgumentException($"Expected a length of {_randomnessBytes}, received {randomness.Length}", nameof(randomness));
            }

            TimeStamp_0 = timestamp.TimeStamp_0;
            TimeStamp_1 = timestamp.TimeStamp_1;
            TimeStamp_2 = timestamp.TimeStamp_2;
            TimeStamp_3 = timestamp.TimeStamp_3;
            TimeStamp_4 = timestamp.TimeStamp_4;
            TimeStamp_5 = timestamp.TimeStamp_5;
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

        public static Uulsid NewUulsid()
        {
            lock (_lock)
            {
                DateTime now = DateTime.UtcNow;
                long timestamp = (long)(now - _epoch).TotalMilliseconds;

                bool isTimestampSameAsLast = timestamp == _lastUsedTimeStamp;
                if (!isTimestampSameAsLast)
                {
                    _lastUsedTimeStamp = timestamp;
                }
                byte[] randomBytes = GetNewRandomBytes(isTimestampSameAsLast);
                randomBytes.CopyTo(_lastUsedRandomness, 0);
                return new Uulsid(new TimestampHelper(timestamp), randomBytes);
            }
        }

        private static byte[] GetNewRandomBytes(bool isTimestampSameAsLast)
        {
            byte[] bytes = new byte[_lastUsedRandomness.Length];
            if (!isTimestampSameAsLast)
            {
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(bytes);
                }
                return bytes;
            }

            _lastUsedRandomness.CopyTo(bytes, 0);
            for (int index = bytes.Length - 1; index >= 0; index--)
            {
                if (bytes[index] < byte.MaxValue)
                {
                    ++bytes[index];
                    return bytes;
                }
                bytes[index] = 0;
            }
            return bytes;
        }

        /// <summary>
        /// Renders this UULSID to a string that can be parsed using either the <see cref="Parse(string)"/> or <see cref="TryParse(string, out Uulsid)"/> methods.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var uulsidCharIndexes = new StringHelper(this);
            var stringChars = new char[StringHelper.TOTAL_LENGTH + 1];
            int i = 0;
            for (; i < StringHelper.TIMESTAMP_LENGTH; i++)
            {
                stringChars[i] = _base32Characters[uulsidCharIndexes[i]];
            }
            stringChars[StringHelper.TIMESTAMP_LENGTH] = '-';
            for (i = StringHelper.TIMESTAMP_LENGTH; i < StringHelper.TOTAL_LENGTH; i++)
            {
                stringChars[i + 1] = _base32Characters[uulsidCharIndexes[i]];
            }
            return new string(stringChars);
        }

        public static bool TryParse(string input, out Uulsid uulsid)
        {
            if (input.Length != StringHelper.TOTAL_LENGTH && input.Length != StringHelper.TOTAL_LENGTH + 1)
            {
                uulsid = default(Uulsid);
                return false;
            }
            int[] index = new int[StringHelper.TOTAL_LENGTH];

            int i = 0;
            int inputOffset = 0;
            do
            {
                char c = char.ToLowerInvariant(input[i + inputOffset]);
                if (c == '-')
                {
                    inputOffset++;
                    continue;
                }
                bool found = false;

                for (int v = 0; v < _base32Characters.Length; ++v)
                {
                    if (_base32Characters[v] == c)
                    {
                        index[i] = v;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    uulsid = default(Uulsid);
                    return false;
                }
                i++;
            } while (i + inputOffset < input.Length);

            uulsid = StringHelper.FromString(index);
            return true;
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

        #region Equality and comparison functions
        public bool Equals(Uulsid other)
        {
            return this.TimeStamp_0 == other.TimeStamp_0
                && this.TimeStamp_1 == other.TimeStamp_1
                && this.TimeStamp_2 == other.TimeStamp_2
                && this.TimeStamp_3 == other.TimeStamp_3
                && this.TimeStamp_4 == other.TimeStamp_4
                && this.TimeStamp_5 == other.TimeStamp_5
                && this.Randomness_0 == other.Randomness_0
                && this.Randomness_1 == other.Randomness_1
                && this.Randomness_2 == other.Randomness_2
                && this.Randomness_3 == other.Randomness_3
                && this.Randomness_4 == other.Randomness_4
                && this.Randomness_5 == other.Randomness_5
                && this.Randomness_6 == other.Randomness_6
                && this.Randomness_7 == other.Randomness_7
                && this.Randomness_8 == other.Randomness_8
                && this.Randomness_9 == other.Randomness_9;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Uulsid))
            {
                return false;
            }
            return Equals((Uulsid)obj);
        }

        public static bool operator ==(Uulsid left, Uulsid right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Uulsid left, Uulsid right)
        {
            return !(left == right);
        }

        public int CompareTo(Uulsid other)
        {
            int result = 0, index = 0;
            var thisHelper = new StringHelper(this);
            var otherHelper = new StringHelper(other);
            while (result == 0 && index < StringHelper.TOTAL_LENGTH)
            {
                result = thisHelper[index].CompareTo(otherHelper[index]);
                ++index;
            }

            return result;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Uulsid))
            {
                throw new ArgumentException($"Expected type {typeof(Uulsid).FullName}, received {obj.GetType().FullName}", nameof(obj));
            }
            return CompareTo((Uulsid)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Of(TimeStamp_0)
                .And(TimeStamp_1)
                .And(TimeStamp_2)
                .And(TimeStamp_3)
                .And(TimeStamp_4)
                .And(TimeStamp_5)
                .And(Randomness_0)
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
        #endregion

        private struct TimestampHelper
        {
            private const long _mask0 = 280375465082880;
            private const long _mask1 = 1095216660480;
            private const long _mask2 = 4278190080;
            private const long _mask3 = 16711680;
            private const long _mask4 = 65280;
            private const long _mask5 = 255;

            public TimestampHelper(long timeStamp)
            {
                TimeStamp_0 = (byte)((timeStamp & _mask0) >> 40);
                TimeStamp_1 = (byte)((timeStamp & _mask1) >> 32);
                TimeStamp_2 = (byte)((timeStamp & _mask2) >> 24);
                TimeStamp_3 = (byte)((timeStamp & _mask3) >> 16);
                TimeStamp_4 = (byte)((timeStamp & _mask4) >> 8);
                TimeStamp_5 = (byte)((timeStamp & _mask5) >> 0);
            }

            public TimestampHelper(byte[] timestamp)
            {
                if (timestamp.Length != _timestampBytes)
                {
                    throw new ArgumentException($"Expected a length of {_timestampBytes}, received {timestamp.Length}", nameof(timestamp));
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

            public long TimeStamp
            {
                get
                {
                    long t0 = TimeStamp_0;
                    long t1 = TimeStamp_1;
                    long t2 = TimeStamp_2;
                    long t3 = TimeStamp_3;
                    long t4 = TimeStamp_4;
                    long t5 = TimeStamp_5;

                    return (t0 << 40)
                        | (t1 << 32)
                        | (t2 << 24)
                        | (t3 << 16)
                        | (t4 << 8)
                        | (t5);
                }
            }
        }

        private struct StringHelper
        {
            public const int TIMESTAMP_LENGTH = 10;
            public const int RANDOMNESS_LENGTH = 16;
            public const int TOTAL_LENGTH = TIMESTAMP_LENGTH + RANDOMNESS_LENGTH;
            public StringHelper(Uulsid value)
            {
                Value = value;
            }

            private Uulsid Value { get; }

            public int this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return (Value.TimeStamp_0 & 224) >> 5;
                        case 1:
                            return Value.TimeStamp_0 & 31;
                        case 2:
                            return (Value.TimeStamp_1 & 248) >> 3;
                        case 3:
                            return ((Value.TimeStamp_1 & 7) << 2) | ((Value.TimeStamp_2 & 192) >> 6);
                        case 4:
                            return (Value.TimeStamp_2 & 62) >> 1;
                        case 5:
                            return ((Value.TimeStamp_2 & 1) << 4) | ((Value.TimeStamp_3 & 240) >> 4);
                        case 6:
                            return ((Value.TimeStamp_3 & 15) << 1) | ((Value.TimeStamp_4 & 128) >> 7);
                        case 7:
                            return (Value.TimeStamp_4 & 124) >> 2;
                        case 8:
                            return ((Value.TimeStamp_4 & 3) << 3) | ((Value.TimeStamp_5 & 224) >> 5);
                        case 9:
                            return Value.TimeStamp_5 & 31;
                        case 10:
                            return (Value.Randomness_0 & 248) >> 3;
                        case 11:
                            return ((Value.Randomness_0 & 7) << 2) | ((Value.Randomness_1 & 192) >> 6);
                        case 12:
                            return (Value.Randomness_1 & 62) >> 1;
                        case 13:
                            return ((Value.Randomness_1 & 1) << 4) | ((Value.Randomness_2 & 240) >> 4);
                        case 14:
                            return ((Value.Randomness_2 & 15) << 1) | ((Value.Randomness_3 & 128) >> 7);
                        case 15:
                            return (Value.Randomness_3 & 124) >> 2;
                        case 16:
                            return ((Value.Randomness_3 & 3) << 3) | ((Value.Randomness_4 & 224) >> 5);
                        case 17:
                            return Value.Randomness_4 & 31;
                        case 18:
                            return (Value.Randomness_5 & 248) >> 3;
                        case 19:
                            return ((Value.Randomness_5 & 7) << 2) | ((Value.Randomness_6 & 192) >> 6);
                        case 20:
                            return (Value.Randomness_6 & 62) >> 1;
                        case 21:
                            return ((Value.Randomness_6 & 1) << 4) | ((Value.Randomness_7 & 240) >> 4);
                        case 22:
                            return ((Value.Randomness_7 & 15) << 1) | ((Value.Randomness_8 & 128) >> 7);
                        case 23:
                            return (Value.Randomness_8 & 124) >> 2;
                        case 24:
                            return ((Value.Randomness_8 & 3) << 3) | ((Value.Randomness_9 & 224) >> 5);
                        case 25:
                            return Value.Randomness_9 & 31;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(index));
                    }
                }
            }

            public static Uulsid FromString(int[] index)
            {
                return new Uulsid(
                    new TimestampHelper(
                        new byte[]
                        {
                            (byte)(index[0] << 5 | index[1]),
                            (byte)(index[2] << 3 | index[3] >> 2),
                            (byte)(index[3] << 6 | index[4] << 1 | index[5] >> 4),
                            (byte)(index[5] << 4 | index[6] >> 1),
                            (byte)(index[6] << 7 | index[7] << 2 | index[8] >> 3),
                            (byte)(index[8] << 5 | index[9])
                        }),
                    new byte[]
                    {
                        (byte)(index[10] << 3 | index[11] >> 2),
                        (byte)(index[11] << 6 | index[12] << 1 | index[13] >> 4),
                        (byte)(index[13] << 4 | index[14] >> 1),
                        (byte)(index[14] << 7 | index[15] << 2 | index[16] >> 3),
                        (byte)(index[16] << 5 | index[17]),
                        (byte)(index[18] << 3 | index[19] >> 2),
                        (byte)(index[19] << 6 | index[20] << 1 | index[21] >> 4),
                        (byte)(index[21] << 4 | index[22] >> 1),
                        (byte)(index[22] << 7 | index[23] << 2 | index[24] >> 3),
                        (byte)(index[24] << 5 | index[25]),
                    }
                );
            }
        }
    }
}
