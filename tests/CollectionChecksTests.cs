using DG.Common.Exceptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace DG.Common.Tests
{
    public class CollectionChecksTests
    {
        public static object[][] EmptyLists = new object[][]
        {
            new object[]
            {
                new List<int>()
            }
        };
        public static object[][] EvensList = new object[][]
        {
            new object[]
            {
                new List<int>() { 2 }
            },
            new object[]
            {
                new List<int>() { 2, 4, 8 }
            }
        };
        public static object[][] OddsList = new object[][]
        {
            new object[]
            {
                new List<int>() { 1 }
            },
            new object[]
            {
                new List<int>() { 1, 3, 5 }
            }
        };

        [Theory]
        [MemberData(nameof(EmptyLists))]
        public void IsEmpty_ShouldThrow(List<int> values)
        {
            Action check = () => ThrowIf.Collection(values, nameof(values)).IsEmpty();

            Assert.Throws<ArgumentException>(check);
        }

        [Theory]
        [MemberData(nameof(EvensList))]
        [MemberData(nameof(OddsList))]
        public void IsEmpty_ShouldNotThrow(List<int> values)
        {
            Action check = () => ThrowIf.Collection(values, nameof(values)).IsEmpty();

            check.Should().NotThrow();
        }

        [Theory]
        [MemberData(nameof(EvensList))]
        public void HasAny_ShouldThrow(List<int> values)
        {
            Func<int, bool> evenCheck = (i) => i % 2 == 0;

            Action check = () => ThrowIf.Collection(values, nameof(values)).Any(evenCheck);

            Assert.Throws<ArgumentException>(check);
        }

        [Theory]
        [MemberData(nameof(EmptyLists))]
        [MemberData(nameof(OddsList))]
        public void HasAny_ShouldNotThrow(List<int> values)
        {
            Func<int, bool> evenCheck = (i) => i % 2 == 0;

            Action check = () => ThrowIf.Collection(values, nameof(values)).Any(evenCheck);

            check.Should().NotThrow();
        }

        [Theory]
        [MemberData(nameof(EmptyLists))]
        [MemberData(nameof(OddsList))]
        public void HasNo_ShouldThrow(List<int> values)
        {
            Func<int, bool> evenCheck = (i) => i % 2 == 0;

            Action check = () => ThrowIf.Collection(values, nameof(values)).None(evenCheck);

            Assert.Throws<ArgumentException>(check);
        }

        [Theory]
        [MemberData(nameof(EvensList))]
        public void HasNo_ShouldNotThrow(List<int> values)
        {
            Func<int, bool> evenCheck = (i) => i % 2 == 0;

            Action check = () => ThrowIf.Collection(values, nameof(values)).None(evenCheck);

            check.Should().NotThrow();
        }
    }
}
