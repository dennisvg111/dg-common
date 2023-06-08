using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DG.Common.Threading
{
    /// <summary>
    /// A class that limits the number of tasks that can be executed asynchronously during a specific interval.
    /// </summary>
    public class RateLimiter
    {
        private readonly TimeSpan _interval;
        private readonly SemaphoreSlim _semaphore;

        /// <summary>
        /// Creates a new instance of <see cref="RateLimiter"/> with a given <paramref name="interval"/>, and an amount <paramref name="maxRequestsPerInterval"/> that can be executed during that interval.
        /// </summary>
        /// <param name="maxRequestsPerInterval"></param>
        /// <param name="interval"></param>
        public RateLimiter(int maxRequestsPerInterval, TimeSpan interval)
        {
            _semaphore = new SemaphoreSlim(maxRequestsPerInterval);
            _interval = interval;
        }

        /// <summary>
        /// Executes the given task when allowed according to this <see cref="RateLimiter"/>, and returns the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> task)
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
                ReleaseSemaphoreAfterDelayAsync(remaining);
            }
        }

        /// <summary>
        /// Executes the given task when allowed according to this <see cref="RateLimiter"/>.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(Func<Task> task)
        {
            Func<Task<int>> function = async () =>
            {
                await task();
                return 1;
            };
            await ExecuteAsync(function);
        }

        /// <summary>
        /// Executes the given task when allowed according to this <see cref="RateLimiter"/>.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(Action task)
        {
            Func<Task<int>> function = async () =>
            {
                await Task.Run(task);
                return 1;
            };
            await ExecuteAsync(function);
        }

        /// <summary>
        /// Executes the given task when allowed according to this <see cref="RateLimiter"/>, and returns the result.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<T> ExecuteAsync<T>(Func<T> task)
        {
            return await ExecuteAsync(() => Task.Run(task));
        }

        private async void ReleaseSemaphoreAfterDelayAsync(TimeSpan remaining)
        {
            if (remaining.TotalMilliseconds > 0)
            {
                await Task.Delay(remaining).ConfigureAwait(false);
            }
            _semaphore.Release();
        }
    }
}
