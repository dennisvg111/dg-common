using DG.Common.Tests.TestHelpers;
using FluentAssertions;
using System;
using Xunit;

namespace DG.Common.Tests
{
    public class UulsidTests
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        [Fact]
        public void Empty_ShouldBeZeroes()
        {
            var uulsid = Uulsid.Empty;

            var s = uulsid.ToString();

            s.Should().Be("0000000000-0000000000000000");
        }

        [Fact]
        public void ToString_SameDate_ShouldStartWithSameCharacters()
        {
            var date = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var milliseconds = Convert.ToInt64((date - _epoch).TotalMilliseconds);
            var A = new Uulsid(milliseconds, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            var B = new Uulsid(milliseconds, new byte[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 });

            A.ToString().Should().StartWith("00vhnczb00-");
            B.ToString().Should().StartWith("00vhnczb00-");

            A.ToString().Should().NotBe(B.ToString());
        }

        [Fact]
        public void ToString_DifferingDates_ShouldBeSortable()
        {
            var date = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var milliseconds = Convert.ToInt64((date - _epoch).TotalMilliseconds);
            var A = new Uulsid(milliseconds, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            var B = new Uulsid(milliseconds + 1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            A.Should().NotBe(B);
            A.Should().BeLessThan(B);

            A.ToString().Should().BeLexographicallyLessThan(B.ToString());
        }

        [Fact]
        public void Equals_OtherNull_DoesNotThrow()
        {
            var uulsid = Uulsid.NewUulsid();

            Action action = () => uulsid.Equals(null);

            action.Should().NotThrow();
        }

        [Fact]
        public void EqualsOperator_LeftNull_DoesNotThrow()
        {
            Uulsid uulsid = null;

            Action action = () =>
            {
                var x = uulsid == Uulsid.Empty;
            };

            action.Should().NotThrow();
        }


        [Fact]
        public void Copy_ReturnsIdentical()
        {
            var uulsid = Uulsid.NewUulsid();

            var copy = new Uulsid(uulsid);

            uulsid.Equals(copy).Should().BeTrue();
            Object.ReferenceEquals(uulsid, copy).Should().BeFalse();
        }
    }
}
