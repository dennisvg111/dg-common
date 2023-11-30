using DG.Common.Threading;
using FluentAssertions;
using Xunit;

namespace DG.Common.Tests.Threading
{
    public class TaskResultTests
    {
        [Fact]
        public void Failure_IsSuccessful_ReturnsFalse()
        {
            var result = TaskResult.Failure<int>();

            result.IsSuccessful.Should().BeFalse();
            result.FailedBecauseOfException.Should().BeFalse();
        }

        [Fact]
        public void Failure_TryGet_ReturnsFalse()
        {
            var result = TaskResult.Failure<int>();

            bool tryGetResult = result.TryGet(out int _);

            tryGetResult.Should().BeFalse();
        }
        [Fact]
        public void Exception_IsSuccessful_ReturnsFalse()
        {
            var result = TaskResult.UnexpectedException<int>();

            result.IsSuccessful.Should().BeFalse();
            result.FailedBecauseOfException.Should().BeTrue();
        }

        [Fact]
        public void Exception_TryGet_ReturnsFalse()
        {
            var result = TaskResult.UnexpectedException<int>();

            bool tryGetResult = result.TryGet(out int _);

            tryGetResult.Should().BeFalse();
        }

        [Fact]
        public void Success_IsSuccessful_ReturnsTrue()
        {
            var result = TaskResult.Success(10);

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Success_TryGet_ReturnsTrue()
        {
            var result = TaskResult.Success(10);

            bool tryGetResult = result.TryGet(out int _);

            tryGetResult.Should().BeTrue();

            result.TryGet(out int value);
            value.Should().Be(10);
        }
    }
}
