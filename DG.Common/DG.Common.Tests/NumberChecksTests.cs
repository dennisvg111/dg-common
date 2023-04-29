using DG.Common.Exceptions;
using DG.Common.Tests.XUnitHelpers;
using System;
using Xunit;

namespace DG.Common.Tests
{
    public class NumberChecksTests
    {
        [Theory]
        [InlineData(0)]
        public void IntIsZero_ShouldThrow(int value)
        {
            Action check = () => ThrowIf.Number(value, nameof(value)).IsZero();

            Assert.Throws<ArgumentException>(check);
        }

        [Theory]
        [InlineData(0)]
        public void ByteIsZero_ShouldThrow(byte value)
        {
            Action check = () => ThrowIf.Number(value, nameof(value)).IsZero();

            Assert.Throws<ArgumentException>(check);
        }

        [Theory]
        [InlineData(0)]
        public void DoubleIsZero_ShouldThrow(double value)
        {
            Action check = () => ThrowIf.Number(value, nameof(value)).IsZero();

            Assert.Throws<ArgumentException>(check);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void IntIsZero_ShouldNotThrow(int value)
        {
            Action check = () => ThrowIf.Number(value, nameof(value)).IsZero();

            MoreAsserts.NoExceptions(check);
        }

        [Theory]
        [InlineData(-0.0001)]
        [InlineData(0.0001)]
        public void DoubleIsZero_ShouldNotThrow(double value)
        {
            Action check = () => ThrowIf.Number(value, nameof(value)).IsZero();

            MoreAsserts.NoExceptions(check);
        }

        [Theory]
        [InlineData(-1)]
        public void IsNegative_ShouldThrow(int value)
        {
            Action check = () => ThrowIf.Number(value, nameof(value)).IsNegative();

            Assert.Throws<ArgumentException>(check);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void IntIsNegative_ShouldNotThrow(int value)
        {
            Action check = () => ThrowIf.Number(value, nameof(value)).IsNegative();

            MoreAsserts.NoExceptions(check);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ByteIsNegative_ShouldNotThrow(byte value)
        {
            Action check = () => ThrowIf.Number(value, nameof(value)).IsNegative();

            MoreAsserts.NoExceptions(check);
        }

        [Theory]
        [InlineData(0.9, 1.0, 10.0)]
        [InlineData(10.1, 1.0, 10.0)]
        [InlineData(-0.9, -1.0, -10.0)]
        [InlineData(-10.1, -1.0, -10.0)]
        public void IsNotBetweenInclusive_ShouldThrow(double value, double min, double max)
        {
            Action check = () => ThrowIf.Number(value, nameof(value)).IsNotBetweenInclusive(min, max);

            Assert.Throws<ArgumentException>(check);
        }

        [Theory]
        [InlineData(1.0, 1.0, 10.0)]
        [InlineData(10.0, 1.0, 10.0)]
        [InlineData(0, -10.0, 10.0)]
        public void IsNotBetweenInclusive_ShouldNotThrow(double value, double min, double max)
        {
            Action check = () => ThrowIf.Number(value, nameof(value)).IsNotBetweenInclusive(min, max);

            MoreAsserts.NoExceptions(check);
        }
    }
}
