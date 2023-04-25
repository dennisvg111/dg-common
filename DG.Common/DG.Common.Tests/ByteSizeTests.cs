using Xunit;

namespace DG.Common.Tests
{
    public class ByteSizeTests
    {
        [Fact]
        public void SingleBytesToString()
        {
            var bytes = ByteSize.FromBytes(10);

            var string1 = bytes.ToString(ByteSize.FormattingStyle.Default);

            Assert.Equal("10 bytes", string1);
        }

        [Fact]
        public void EqualsWorks()
        {
            var bytes1 = ByteSize.FromBytes(56);
            var bytes2 = ByteSize.FromBytes(55);
            var bytes3 = ByteSize.FromBytes(56);

            Assert.NotEqual(bytes1, bytes2);
            Assert.Equal(bytes1, bytes3);
        }

        [Fact]
        public void EqualsOperatorWorks()
        {
            var bytes1 = ByteSize.FromBytes(24);
            var bytes2 = ByteSize.FromBytes(24);
            var bytes3 = ByteSize.FromBytes(56);

            Assert.True(bytes1 == bytes2);
            Assert.True(bytes1 != bytes3);
        }

        [Fact]
        public void ByteArrayReturnsLength()
        {
            var byteArray = new byte[] { 1, 2, 3 };
            var expected = ByteSize.FromBytes(3);

            var bytes2 = ByteSize.FromBytes(byteArray);

            Assert.Equal(expected, bytes2);
        }

        [Fact]
        public void BinaryInstance()
        {
            var expected = ByteSize.FromBytes(3_145_728);

            var bytes1 = ByteSize.FromMB(3, ByteSize.UnitType.Binary);

            Assert.Equal(expected, bytes1);
        }

        [Fact]
        public void DecimalInstance()
        {
            var expected = ByteSize.FromBytes(3_000_000);

            var bytes1 = ByteSize.FromMB(3, ByteSize.UnitType.Decimal);

            Assert.Equal(expected, bytes1);
        }

        [Fact]
        public void AddEqualToZeroPlusTotal()
        {
            var bytes1 = ByteSize.FromKB(1);
            var bytes2 = ByteSize.FromBytes(0).AddKB(1);

            Assert.Equal(bytes2, bytes1);
        }
    }
}
