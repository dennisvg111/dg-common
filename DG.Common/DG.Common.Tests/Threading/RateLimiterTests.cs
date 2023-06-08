using DG.Common.Tests.XUnitHelpers;
using DG.Common.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DG.Common.Tests.Threading
{
    public class RateLimiterTests
    {
        private const int _maxRequestsPerInterval = 5;
        private static readonly TimeSpan _interval = TimeSpan.FromSeconds(3);

        private const int _amountOfRequests = 15;

        [Fact]
        public async void Execute_CanRunParallel()
        {
            var results = await GetRateLimitedOffsets();
            var offsets = results.OrderBy(r => r.Offset).ToArray();

            Assert.InRange(offsets[0].Offset, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2));
            if (_amountOfRequests > 1)
            {
                MoreAsserts.IsInMargin(offsets[1].Offset, offsets[0].Offset, TimeSpan.FromMilliseconds(10));
            }
        }

        [Fact]
        public async void Execute_WaitsBeforeRatelimit()
        {
            int expectedCompleteGroups = _amountOfRequests / _maxRequestsPerInterval;
            var results = await GetRateLimitedOffsets();
            var offsets = results.OrderBy(r => r.Offset).ToArray();

            for (int i = 0; i < expectedCompleteGroups; i++)
            {
                TimeSpan extraOffset = TimeSpan.FromTicks(i * _interval.Ticks);
                Assert.Equal(_maxRequestsPerInterval, offsets.Count(t => t.Offset >= TimeSpan.FromSeconds(0) + extraOffset && t.Offset <= TimeSpan.FromSeconds(1) + extraOffset));
            }
        }

        [Fact(Skip = "Not important for now")]
        public async void Execute_Fifo()
        {
            var results = await GetRateLimitedOffsets();

            var offsets = results.OrderBy(t => t.Started).Select(r => r.Offset).ToArray();
            MoreAsserts.IsOrdered(offsets);
        }

        private static async Task<TimerResult[]> GetRateLimitedOffsets()
        {
            var limiter = new RateLimiter(5, _interval);

            List<Task<TimerResult>> tasks = new List<Task<TimerResult>>();
            for (int i = 0; i < _amountOfRequests; i++)
            {
                var result = new TimerResult();
                tasks.Add(limiter.WaitFor(() => result.SetFinished()));
            }

            return await Task.WhenAll(tasks);
        }

        private class TimerResult
        {
            private readonly DateTime _started = DateTime.Now;
            private TimeSpan _offset;

            public DateTime Started => _started;
            public TimeSpan Offset => _offset;

            public async Task<TimerResult> SetFinished()
            {
                _offset = DateTime.Now - _started;
                await Task.Delay(1000);
                return this;
            }

            public override string ToString()
            {
                return _offset.ToString();
            }
        }
    }
}
