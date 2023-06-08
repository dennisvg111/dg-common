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
        public async void WaitFor_CanRunParallel()
        {
            var results = await ExecuteTestTasks();
            var timeouts = results.OrderBy(r => r.RateLimitedFor).ToArray();

            Assert.InRange(timeouts[0].RateLimitedFor, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2));
            if (_amountOfRequests > 1)
            {
                MoreAsserts.IsInMargin(timeouts[1].RateLimitedFor, timeouts[0].RateLimitedFor, TimeSpan.FromMilliseconds(10));
            }
        }

        [Fact]
        public async void WaitFor_WaitsBeforeRatelimit()
        {
            int expectedCompleteGroups = _amountOfRequests / _maxRequestsPerInterval;
            var results = await ExecuteTestTasks();
            var timeouts = results.OrderBy(r => r.RateLimitedFor).ToArray();

            for (int i = 0; i < expectedCompleteGroups; i++)
            {
                TimeSpan extraOffset = TimeSpan.FromTicks(i * _interval.Ticks);
                var timeoutsInTimespan = timeouts.Count(t => t.RateLimitedFor >= TimeSpan.FromSeconds(-0.01) + extraOffset && t.RateLimitedFor <= TimeSpan.FromSeconds(1) + extraOffset);
                Assert.True(timeoutsInTimespan == _maxRequestsPerInterval, $"Expected {_maxRequestsPerInterval} tasks rate-limited for around {extraOffset}, actual {timeoutsInTimespan}. Total range {timeouts.Select(t => t.RateLimitedFor).Min()}-{timeouts.Select(t => t.RateLimitedFor).Max()}.");
            }
        }

        [Fact(Skip = "Not important for now")]
        public async void WaitFor_FirstInFirstOut()
        {
            var results = await ExecuteTestTasks();

            var timeouts = results.OrderBy(t => t.TaskStartTime).Select(r => r.RateLimitedFor).ToArray();
            MoreAsserts.IsOrdered(timeouts);
        }

        private static async Task<RateLimiterResult[]> ExecuteTestTasks()
        {
            var limiter = new RateLimiter(5, _interval);

            List<Task<RateLimiterResult>> tasks = new List<Task<RateLimiterResult>>();
            for (int i = 0; i < _amountOfRequests; i++)
            {
                var result = new RateLimiterResult();
                tasks.Add(limiter.ExecuteAsync(() => result.SetActualStartTime()));
            }

            return await Task.WhenAll(tasks);
        }

        private class RateLimiterResult
        {
            private readonly DateTime _taskStartTime = DateTime.Now;
            private DateTime _actualStartTime;

            public DateTime TaskStartTime => _taskStartTime;
            public TimeSpan RateLimitedFor => _actualStartTime - _taskStartTime;

            public async Task<RateLimiterResult> SetActualStartTime()
            {
                _actualStartTime = DateTime.Now;

                //wait so we can test if the rate limiter takes task time into account.
                await Task.Delay(1000);
                return this;
            }

            public override string ToString()
            {
                return RateLimitedFor.ToString();
            }
        }
    }
}
