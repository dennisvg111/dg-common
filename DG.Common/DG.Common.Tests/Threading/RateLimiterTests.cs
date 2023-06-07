using DG.Common.Tests.XUnitHelpers;
using DG.Common.Threading;
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
        public void Execute_CanRunParallel()
        {
            var offsets = GetRateLimitedOffsets(10);
            offsets = offsets.OrderBy(r => r.Started).ToArray();

            Assert.InRange(offsets[0].Offset, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2));
            MoreAsserts.IsInMargin(offsets[1].Offset, offsets[0].Offset, TimeSpan.FromMilliseconds(10));
        }

        [Fact]
        public void Execute_WaitsBeforeRatelimit()
        {
            var offsets = GetRateLimitedOffsets(15);
            offsets = offsets.OrderBy(r => r.Started).ToArray();

            Assert.Equal(5, offsets.Count(t => t.Offset >= TimeSpan.FromSeconds(0) && t.Offset <= TimeSpan.FromSeconds(1)));
            Assert.Equal(5, offsets.Count(t => t.Offset >= TimeSpan.FromSeconds(0) + _interval && t.Offset <= TimeSpan.FromSeconds(1) + _interval));
        }

        [Fact]
        public void Execute_Fifo()
        {
            var results = GetRateLimitedOffsets(15);

            var offsets = results.OrderBy(t => t.Started).Select(r => r.Offset).ToArray();
            MoreAsserts.IsOrdered(offsets);
        }

        private static TimerResult[] GetRateLimitedOffsets(int count)
        {
            var limiter = new RateLimiter(5, _interval);

            DateTime start = DateTime.Now;
            TimerResult[] offsets = new TimerResult[count];

            Parallel.For(0, offsets.Length, (i) =>
            {
                offsets[i] = new TimerResult();
                var time = limiter.WaitFor(() => GetTime()).Result;
                offsets[i].SetFinished(time - start);
            });

            return offsets;
        }

        private static async Task<DateTime> GetTime()
        {
            var started = DateTime.Now;
            await Task.Delay(1000);
            return started;
        }

        private class TimerResult
        {
            private readonly DateTime _started = DateTime.Now;
            private TimeSpan _offset;

            public DateTime Started => _started;
            public TimeSpan Offset => _offset;

            public void SetFinished(TimeSpan offset)
            {
                _offset = offset;
            }

            public override string ToString()
            {
                return _offset.ToString();
            }
        }
    }
}
