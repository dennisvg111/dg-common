using FluentAssertions;
using System;
using System.Globalization;
using Xunit;

namespace DG.Common.Tests
{
    public class ByteSizeTests
    {
        private const int _kibiByte = 1024;
        private const long _mebiByte = _kibiByte * _kibiByte;
        private const long _gibiByte = _mebiByte * _kibiByte;

        private const int _kiloByte = 1000;
        private const long _megaByte = _kiloByte * _kiloByte;
        private const long _gigaByte = _megaByte * _kiloByte;

        private static readonly IFormatProvider _nlCulture = new CultureInfo("nl-NL");

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1024)]
        [InlineData(1023)]
        [InlineData(999)]
        [InlineData(long.MaxValue)]
        [InlineData(long.MinValue)]
        public void TotalBytes_CreatedWithFromBytes_Works(long byteCount)
        {
            var bytes = ByteSize.FromBytes(byteCount);

            bytes.TotalBytes.Should().Be(byteCount);
        }

        [Fact]
        public void Add_Works()
        {
            var bytes = ByteSize.FromGB(4).AddMB(150);

            bytes.TotalBytes.Should().Be(4_452_253_696L);
        }

        [Fact]
        public void Equals_Works()
        {
            var bytes1 = ByteSize.FromBytes(56);
            var bytes2 = ByteSize.FromBytes(55);
            var bytes3 = ByteSize.FromBytes(56);

            bytes1.Equals(bytes2).Should().BeFalse();
            bytes1.Equals(bytes3).Should().BeTrue();
        }

        [Fact]
        public void EqualsOperator_Works()
        {
            var bytes1 = ByteSize.FromBytes(24);
            var bytes2 = ByteSize.FromBytes(24);
            var bytes3 = ByteSize.FromBytes(56);

            (bytes1 == bytes2).Should().BeTrue();
            (bytes1 == bytes3).Should().BeFalse();
        }

        [Fact]
        public void NotEqualsOperator_Works()
        {
            var bytes1 = ByteSize.FromBytes(24);
            var bytes2 = ByteSize.FromBytes(24);
            var bytes3 = ByteSize.FromBytes(56);

            (bytes1 != bytes2).Should().BeFalse();
            (bytes1 != bytes3).Should().BeTrue();
        }

        [Fact]
        public void FromBytes_ByteArray_Works()
        {
            var byteArray = new byte[] { 1, 2, 3 };

            var bytes = ByteSize.FromBytes(byteArray);

            bytes.TotalBytes.Should().Be(3);
        }

        [Fact]
        public void FromMB_Binary_Works()
        {
            var bytes = ByteSize.FromMB(3, ByteSize.UnitType.Binary);

            bytes.TotalBytes.Should().Be(3_145_728);
        }

        [Fact]
        public void FromMB_Decimal_Works()
        {
            var bytes = ByteSize.FromMB(3, ByteSize.UnitType.Decimal);

            bytes.TotalBytes.Should().Be(3_000_000);
        }

        [Fact]
        public void AddKB_ChangesTotalBytes()
        {
            var bytes2 = ByteSize.FromBytes(0).AddKB(1);

            bytes2.TotalBytes.Should().Be(1024);
        }

        [Fact]
        public void ImplicitOperator_Works()
        {
            long bytes = ByteSize.FromMB(1).AddKB(200);

            bytes.Should().Be(1_253_376L);
        }

        [Theory]
        [InlineData(0, "0 B")]
        [InlineData(1000, "1000 B")]

        [InlineData(_kibiByte, "1 KB")]
        [InlineData(_kibiByte * 1.5, "1.5 KB")]
        [InlineData(_kibiByte * 1000, "1000 KB")]
        [InlineData(_mebiByte - 1, "1024 KB")]

        [InlineData(_mebiByte, "1 MB")]
        [InlineData(_mebiByte * 1.5, "1.5 MB")]
        [InlineData(_mebiByte * 1000, "1000 MB")]
        [InlineData(_gibiByte - 1, "1024 MB")]

        [InlineData(long.MaxValue, "8 EB")]
        public void ToString_Default(long bytes, string expected)
        {
            var size = ByteSize.FromBytes(bytes);

            var defaultToString = size.ToString();
            var explicitToString = size.ToString(ByteSize.FormattingStyle.Default);
            var nlString = size.ToString(ByteSize.FormattingStyle.Default, _nlCulture);

            defaultToString.ToString().Should().Be(expected);
            explicitToString.Should().Be(expected);

            var nlExpected = expected.Replace(".", ",");
            nlString.Should().Be(nlExpected);
        }

        [Theory]
        [InlineData(0, "0 B")]
        [InlineData(1000, "1000 B")]

        [InlineData(_kibiByte, "1 KiB")]
        [InlineData(_kibiByte * 1.5, "1.5 KiB")]
        [InlineData(_kibiByte * 1000, "1000 KiB")]
        [InlineData(_mebiByte - 1, "1024 KiB")]

        [InlineData(_mebiByte, "1 MiB")]
        [InlineData(_mebiByte * 1.5, "1.5 MiB")]
        [InlineData(_mebiByte * 1000, "1000 MiB")]
        [InlineData(_gibiByte - 1, "1024 MiB")]

        [InlineData(long.MaxValue, "8 EiB")]
        public void ToString_IecBinary(long bytes, string expected)
        {
            var size = ByteSize.FromBytes(bytes);

            var explicitToString = size.ToString(ByteSize.FormattingStyle.IecBinary);
            var nlString = size.ToString(ByteSize.FormattingStyle.IecBinary, _nlCulture);

            explicitToString.Should().Be(expected);

            var nlExpected = expected.Replace(".", ",");
            nlString.Should().Be(nlExpected);
        }

        [Theory]
        [InlineData(0, "0 B")]
        [InlineData(999, "999 B")]

        [InlineData(_kiloByte, "1 kB")]
        [InlineData(_kiloByte * 1.5, "1.5 kB")]
        [InlineData(_megaByte - 1, "1000 kB")]

        [InlineData(_megaByte, "1 mB")]
        [InlineData(_megaByte * 1.5, "1.5 mB")]
        [InlineData(_gigaByte - 1, "1000 mB")]

        [InlineData(long.MaxValue, "9.22 eB")]
        public void ToString_IecDecimal(long bytes, string expected)
        {
            var size = ByteSize.FromBytes(bytes);

            var explicitToString = size.ToString(ByteSize.FormattingStyle.IecDecimal);
            var nlString = size.ToString(ByteSize.FormattingStyle.IecDecimal, _nlCulture);

            explicitToString.Should().Be(expected);

            var nlExpected = expected.Replace(".", ",");
            nlString.Should().Be(nlExpected);
        }
    }
}