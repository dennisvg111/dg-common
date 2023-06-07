using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DG.Common.Threading
{
    public class RateLimiter
    {
        private readonly int _maxRequestsPerInterval;
        private readonly TimeSpan _interval;
        private readonly SemaphoreSlim _semaphore;

        public RateLimiter(int maxRequestsPerInterval, TimeSpan interval)
        {
            _semaphore = new SemaphoreSlim(maxRequestsPerInterval);
            _maxRequestsPerInterval = maxRequestsPerInterval;
            _interval = interval;
        }

        public async Task<T> WaitFor<T>(Func<Task<T>> task)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                return await task().ConfigureAwait(false);
            }
            finally
            {
                var remaining = _interval - stopwatch.Elapsed;
                _ = ReleaseSemaphoreAfterDelayAsync(remaining);
            }
        }

        private async Task ReleaseSemaphoreAfterDelayAsync(TimeSpan remaining)
        {
            if (remaining.TotalMilliseconds > 0)
            {
                await Task.Delay(remaining);
            }
            _semaphore.Release();
        }
    }
}
