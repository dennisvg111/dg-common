﻿using FluentAssertions;
using Xunit;

namespace DG.Common.Tests
{
    public class HashCodeTests
    {
        [Fact]
        public void SameObjectGivesSameHashcode()
        {
            string string1 = "abc";
            string string2 = "abc";

            var hashcode1 = HashCode.Of(string1);
            var hashcode2 = HashCode.Of(string2);

            hashcode1.Should().Be(hashcode2);
        }

        [Fact]
        public void GetHashCode_SameAsValue()
        {
            string string1 = "test";

            HashCode hashcode = HashCode.Of(string1);

            int codeAsInt = hashcode;
            int hashcodeResult = hashcode.GetHashCode();

            codeAsInt.Should().Be(hashcodeResult);
        }

        [Fact]
        public void OrderShouldMatter()
        {
            string string1 = "abc";
            string string2 = "def";

            var hashcode1 = HashCode.Of(string1).And(string2);
            var hashcode2 = HashCode.Of(string2).And(hashcode1);

            hashcode1.Should().NotBe(hashcode2);
        }
    }
}
