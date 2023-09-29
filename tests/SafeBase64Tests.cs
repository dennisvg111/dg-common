using Xunit;

namespace DG.Common.Tests
{
    public class SafeBase64Tests
    {
        [Theory]
        [InlineData("eyJz", new byte[] { 123, 34, 115 })]
        [InlineData("-yJz", new byte[] { 251, 34, 115 })]
        [InlineData("e_Jz", new byte[] { 123, 242, 115 })]
        [InlineData("eyJzdWIiOiii", new byte[] { 123, 34, 115, 117, 98, 34, 58, 40, 162 })] //0 trimmed '=' characters
        [InlineData("eyJzdWIiOig", new byte[] { 123, 34, 115, 117, 98, 34, 58, 40 })] //1 trimmed '=' characters
        [InlineData("eyJzdWIiOg", new byte[] { 123, 34, 115, 117, 98, 34, 58 })] //2 trimmed '=' characters
        public void Decode_Works(string base64, byte[] expected)
        {
            var result = SafeBase64.Decode(base64);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(new byte[] { 123, 34, 115 }, "eyJz")]
        [InlineData(new byte[] { 251, 34, 115 }, "-yJz")]
        [InlineData(new byte[] { 123, 242, 115 }, "e_Jz")]
        [InlineData(new byte[] { 123, 34, 115, 117, 98, 34 }, "eyJzdWIi")] //0 trimmed '=' characters
        [InlineData(new byte[] { 123, 34, 115, 117, 98 }, "eyJzdWI")] //1 trimmed '=' characters
        [InlineData(new byte[] { 123, 34, 115, 117 }, "eyJzdQ")] //2 trimmed '=' characters
        public void Encode_Works(byte[] bytes, string expected)
        {
            var result = SafeBase64.Encode(bytes);

            Assert.Equal(expected, result);
        }
    }
}
