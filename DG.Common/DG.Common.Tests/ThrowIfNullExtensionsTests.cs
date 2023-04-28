using DG.Common.Exceptions;
using DG.Common.Tests.XUnitHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace DG.Common.Tests
{
    public class ThrowIfNullExtensionsTests
    {
        public static IEnumerable<object[]> DefaultValueData => new List<object[]>
        {
            new object[] { default(bool), "bool" },
            new object[] { default(int), "int" },
            new object[] { default(Guid), "guid" },
            new object[] { default(DateTime), "datetime" },
        };
        public static IEnumerable<object[]> FilledCollectionData => new List<object[]>
        {
            new object[] { new List<int>() { 1, 3}, "list" },
            new object[] { new int[] { 1 }, "array" },
        };
        public static IEnumerable<object[]> EmptyCollectionData => new List<object[]>
        {
            new object[] { new List<int>(), "list" },
            new object[] { new int[0], "array" },
        };
        public static IEnumerable<object[]> NullValueData => new List<object[]>
        {
            new object[] { (string)null, "string" },
            new object[] { (int?)null, "int" },
            new object[] { null, "object" }
        };

        [Theory]
        [MemberData(nameof(DefaultValueData))]
        [MemberData(nameof(EmptyCollectionData))]
        [MemberData(nameof(FilledCollectionData))]
        [InlineData("", "string")]
        public void Null_ShouldNotThrow<T>(T input, string name)
        {
            MoreAsserts.NoExceptions(() => Throws.If.Null(input, name));
        }

        [Theory]
        [MemberData(nameof(NullValueData))]
        public void Null_ShouldThrowArgumentNullException<T>(T input, string name)
        {
            Assert.Throws<ArgumentNullException>(() => Throws.If.Null(input, name));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Null_MessageShouldNotBeNullOrEmpty(string message)
        {
            string input = null;

            var exception = Assert.Throws<ArgumentNullException>(() => Throws.If.Null(input, nameof(input), message));

            Assert.NotNull(exception.Message);
            Assert.NotEmpty(exception.Message);
        }

        [Fact]
        public void Null_MessageCanBeGiven()
        {
            string input = null;
            string message = "This is a custom message";

            var exception = Assert.Throws<ArgumentNullException>(() => Throws.If.Null(input, nameof(input), message));

            Assert.NotNull(exception.Message);
            Assert.Contains(message, exception.Message);
        }

        [Fact]
        public void Null_ArgumentNameCanBeGiven()
        {
            string input = null;
            string argumentName = "inputString";

            var exception = Assert.Throws<ArgumentNullException>(() => Throws.If.Null(input, argumentName));

            Assert.NotNull(exception.ParamName);
            Assert.Equal(argumentName, exception.ParamName);
        }

        [Theory]
        [MemberData(nameof(DefaultValueData))]
        public void NullOrDefault_ShouldThrowArgumentExceptionOnDefault<T>(T input, string name)
        {
            Assert.Throws<ArgumentException>(() => Throws.If.NullOrDefault(input, name));
        }

        [Fact]
        public void NullOrDefault_ShouldNotThrowArgumentExceptionOnNotDefault()
        {
            Throws.If.NullOrDefault("a", "string");
            Throws.If.NullOrDefault(1, "int");
            Throws.If.NullOrDefault(DateTime.Now, "datetime");
        }

        [Theory]
        [MemberData(nameof(EmptyCollectionData))]
        public void NullOrEmpty_ShouldThrowArgumentExceptionOnEmpty<T>(IEnumerable<T> input, string name)
        {
            Assert.Throws<ArgumentException>(() => Throws.If.NullOrEmpty(input, name));
        }

        [Fact]
        public void NullOrEmpty_ShouldThrowArgumentExceptionOnEmptyDictionary()
        {
            NullOrEmpty_ShouldThrowArgumentExceptionOnEmpty(new Dictionary<int, string>(), "dictionary");
        }

        [Theory]
        [MemberData(nameof(FilledCollectionData))]
        public void NullOrEmpty_ShouldNotThrowArgumentExceptionOnFilled<T>(IEnumerable<T> input, string name)
        {
            MoreAsserts.NoExceptions(() => Throws.If.NullOrEmpty(input, name));
        }

        [Fact]
        public void NullOrEmpty_ShouldNotThrowArgumentExceptionOnFilledDictionary()
        {
            NullOrEmpty_ShouldNotThrowArgumentExceptionOnFilled(new Dictionary<int, string>() { { 10, "name" } }, "dictionary");
        }

        [Fact]
        public void NullOrEmpty_ShouldThrowOnNullString()
        {
            Assert.Throws<ArgumentNullException>(() => Throws.If.NullOrEmpty(null, "string"));
        }

        [Fact]
        public void NullOrEmpty_ShouldThrowOnEmptyString()
        {
            Assert.Throws<ArgumentException>(() => Throws.If.NullOrEmpty(string.Empty, "string"));
        }

        [Fact]
        public void NullOrEmpty_ShouldNotThrowOnFilledString()
        {
            MoreAsserts.NoExceptions(() => Throws.If.NullOrEmpty("-", "string"));
        }

        [Fact]
        public void NullOrWhitespace_ShouldThrowOnNullString()
        {
            Assert.Throws<ArgumentNullException>(() => Throws.If.NullOrWhiteSpace(null, "string"));
        }

        [Fact]
        public void NullOrWhitespace_ShouldThrowOnEmptyString()
        {
            Assert.Throws<ArgumentException>(() => Throws.If.NullOrWhiteSpace(string.Empty, "string"));
        }

        [Fact]
        public void NullOrWhitespace_ShouldThrowOnWhitespaceString()
        {
            Assert.Throws<ArgumentException>(() => Throws.If.NullOrWhiteSpace("\t \r\n", "string"));
        }

        [Fact]
        public void NullOrWhitespace_ShouldNotThrowOnFilledString()
        {
            MoreAsserts.NoExceptions(() => Throws.If.NullOrWhiteSpace("\t- \r\n", "string"));
        }
    }
}
