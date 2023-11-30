using DG.Common.Tests.Threading.Helpers;
using DG.Common.Threading;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DG.Common.Tests.Threading
{
    public class RateLimiterTests
    {
        private const int _maxRequestsPerInterval = 5;
        private static readonly TimeSpan _interval = TimeSpan.FromSeconds(3);

        [Fact]
        public async Task ExecuteAsync_CanRunParallel()
        {
            var limiter = new RateLimiter(_maxRequestsPerInterval, _interval);

            var results = await limiter.ExecuteNFunctionsAsync(3);
            var timeouts = results.OrderBy(r => r.RateLimitedFor).ToArray();

            timeouts[0].RateLimitedFor.Should().BeCloseTo(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(10));
            timeouts[1].RateLimitedFor.Should().BeCloseTo(timeouts[0].RateLimitedFor, TimeSpan.FromMilliseconds(10));
        }

        [Fact]
        public async Task ExecuteAsync_WaitsBeforeRatelimit()
        {
            var limiter = new RateLimiter(_maxRequestsPerInterval, _interval);

            var results = await limiter.ExecuteNFunctionsAsync(17);
            var timeouts = results.OrderBy(r => r.RateLimitedFor).ToArray();

            int expectedCompleteGroups = results.Length / _maxRequestsPerInterval;
            for (int i = 0; i < expectedCompleteGroups; i++)
            {
                TimeSpan extraOffset = TimeSpan.FromTicks(i * _interval.Ticks);
                var timeoutsInTimespan = timeouts.Count(t => t.RateLimitedFor >= TimeSpan.FromSeconds(-0.01) + extraOffset && t.RateLimitedFor <= TimeSpan.FromSeconds(1) + extraOffset);

                var reason = $"Expected {_maxRequestsPerInterval} tasks rate-limited for around {extraOffset}, actual {timeoutsInTimespan}. Total range {timeouts.Select(t => t.RateLimitedFor).Min()}-{timeouts.Select(t => t.RateLimitedFor).Max()}.";
                timeoutsInTimespan.Should().Be(_maxRequestsPerInterval, reason);
            }
        }

        [Fact(Skip = "Not important for now")]
        public async Task ExecuteAsync_FirstInFirstOut()
        {
            var limiter = new RateLimiter(_maxRequestsPerInterval, _interval);
            var results = await limiter.ExecuteNFunctionsAsync(20);

            var timeouts = results.OrderBy(t => t.TaskStartTime).Select(r => r.RateLimitedFor).ToArray();
            timeouts.Should().BeInAscendingOrder();
        }
    }
}
