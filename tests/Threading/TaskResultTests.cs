using DG.Common.Threading;
using Xunit;

namespace DG.Common.Tests.Threading
{
    public class TaskResultTests
    {
        [Fact]
        public void Failure_IsSuccessful_ReturnsFalse()
        {
            var result = TaskResult.Failure<int>();

            Assert.False(result.IsSuccessful);
            Assert.False(result.FailedBecauseOfException);
        }

        [Fact]
        public void Failure_TryGet_ReturnsFalse()
        {
            var result = TaskResult.Failure<int>();

            Assert.False(result.TryGet(out int _));
        }
        [Fact]
        public void Exception_IsSuccessful_ReturnsFalse()
        {
            var result = TaskResult.UnexpectedException<int>();

            Assert.False(result.IsSuccessful);
            Assert.True(result.FailedBecauseOfException);
        }

        [Fact]
        public void Exception_TryGet_ReturnsFalse()
        {
            var result = TaskResult.UnexpectedException<int>();

            Assert.False(result.TryGet(out int _));
        }

        [Fact]
        public void Success_IsSuccessful_ReturnsTrue()
        {
            var result = TaskResult.Success(10);

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void Success_TryGet_ReturnsTrue()
        {
            var result = TaskResult.Success(10);

            Assert.True(result.TryGet(out int _));

            result.TryGet(out int value);
            Assert.Equal(10, value);
        }
    }
}
