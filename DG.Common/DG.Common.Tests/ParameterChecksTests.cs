using DG.Common.Exceptions;
using DG.Common.Tests.XUnitHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace DG.Common.Tests
{
    public class ParameterChecksTests
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
            MoreAsserts.NoExceptions(() => ThrowIf.Parameter.IsNull(input, name));
        }

        [Theory]
        [MemberData(nameof(NullValueData))]
        public void Null_ShouldThrowArgumentNullException<T>(T input, string name)
        {
            Assert.Throws<ArgumentNullException>(() => ThrowIf.Parameter.IsNull(input, name));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Null_MessageShouldNotBeNullOrEmpty(string message)
        {
            string input = null;

            var exception = Assert.Throws<ArgumentNullException>(() => ThrowIf.Parameter.IsNull(input, nameof(input), message));

            Assert.NotNull(exception.Message);
            Assert.NotEmpty(exception.Message);
        }

        [Fact]
        public void Null_MessageCanBeGiven()
        {
            string input = null;
            string message = "This is a custom message";

            var exception = Assert.Throws<ArgumentNullException>(() => ThrowIf.Parameter.IsNull(input, nameof(input), message));

            Assert.NotNull(exception.Message);
            Assert.Contains(message, exception.Message);
        }

        [Fact]
        public void Null_ArgumentNameCanBeGiven()
        {
            string input = null;
            string argumentName = "inputString";

            var exception = Assert.Throws<ArgumentNullException>(() => ThrowIf.Parameter.IsNull(input, argumentName));

            Assert.NotNull(exception.ParamName);
            Assert.Equal(argumentName, exception.ParamName);
        }

        [Theory]
        [MemberData(nameof(DefaultValueData))]
        public void NullOrDefault_ShouldThrowArgumentExceptionOnDefault<T>(T input, string name)
        {
            Assert.Throws<ArgumentException>(() => ThrowIf.Parameter.IsNullOrDefault(input, name));
        }

        [Fact]
        public void NullOrDefault_ShouldNotThrowArgumentExceptionOnNotDefault()
        {
            ThrowIf.Parameter.IsNullOrDefault("a", "string");
            ThrowIf.Parameter.IsNullOrDefault(1, "int");
            ThrowIf.Parameter.IsNullOrDefault(DateTime.Now, "datetime");
        }

        [Theory]
        [MemberData(nameof(EmptyCollectionData))]
        public void NullOrEmpty_ShouldThrowArgumentExceptionOnEmpty<T>(IEnumerable<T> input, string name)
        {
            Assert.Throws<ArgumentException>(() => ThrowIf.Parameter.IsNullOrEmpty(input, name));
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
            MoreAsserts.NoExceptions(() => ThrowIf.Parameter.IsNullOrEmpty(input, name));
        }

        [Fact]
        public void NullOrEmpty_ShouldNotThrowArgumentExceptionOnFilledDictionary()
        {
            NullOrEmpty_ShouldNotThrowArgumentExceptionOnFilled(new Dictionary<int, string>() { { 10, "name" } }, "dictionary");
        }

        [Fact]
        public void NullOrEmpty_ShouldThrowOnNullString()
        {
            Assert.Throws<ArgumentNullException>(() => ThrowIf.Parameter.IsNullOrEmpty(null, "string"));
        }

        [Fact]
        public void NullOrEmpty_ShouldThrowOnEmptyString()
        {
            Assert.Throws<ArgumentException>(() => ThrowIf.Parameter.IsNullOrEmpty(string.Empty, "string"));
        }

        [Fact]
        public void NullOrEmpty_ShouldNotThrowOnFilledString()
        {
            MoreAsserts.NoExceptions(() => ThrowIf.Parameter.IsNullOrEmpty("-", "string"));
        }

        [Fact]
        public void NullOrWhitespace_ShouldThrowOnNullString()
        {
            Assert.Throws<ArgumentNullException>(() => ThrowIf.Parameter.IsNullOrWhiteSpace(null, "string"));
        }

        [Fact]
        public void NullOrWhitespace_ShouldThrowOnEmptyString()
        {
            Assert.Throws<ArgumentException>(() => ThrowIf.Parameter.IsNullOrWhiteSpace(string.Empty, "string"));
        }

        [Fact]
        public void NullOrWhitespace_ShouldThrowOnWhitespaceString()
        {
            Assert.Throws<ArgumentException>(() => ThrowIf.Parameter.IsNullOrWhiteSpace("\t \r\n", "string"));
        }

        [Fact]
        public void NullOrWhitespace_ShouldNotThrowOnFilledString()
        {
            MoreAsserts.NoExceptions(() => ThrowIf.Parameter.IsNullOrWhiteSpace("\t- \r\n", "string"));
        }


        [Fact]
        public void Matches_ShouldNotThrowOnFalse()
        {
            int i = 0;

            Func<int, bool> predicate = (x) => false;

            MoreAsserts.NoExceptions(() => ThrowIf.Parameter.Matches(i, predicate, nameof(i)));
        }
        [Fact]
        public void Matches_ShouldNotThrowOnNullAndFalse()
        {
            object obj = null;

            Func<object, bool> predicate = (x) => false;

            MoreAsserts.NoExceptions(() => ThrowIf.Parameter.Matches(obj, predicate, nameof(obj)));
        }

        [Fact]
        public void Matches_ShouldThrowOnTrue()
        {
            int i = 0;

            Func<int, bool> predicate = (x) => true;

            Assert.Throws<ArgumentException>(() => ThrowIf.Parameter.Matches(i, predicate, nameof(i)));
        }

        [Fact]
        public void Matches_ShouldThrowOnNullAndTrue()
        {
            object obj = null;

            Func<object, bool> predicate = (x) => true;

            Assert.Throws<ArgumentException>(() => ThrowIf.Parameter.Matches(obj, predicate, nameof(obj)));
        }
    }
}
