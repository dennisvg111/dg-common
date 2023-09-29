using System;
using System.Threading;
using System.Threading.Tasks;

namespace DG.Common.Threading
{
    /// <summary>
    /// Provides functionality for locking during async await operations.
    /// </summary>
    public class TaskLock
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Executes the given <see cref="Task"/> on only a single thread at a time.
        /// </summary>
        /// <param name="worker"></param>
        /// <returns></returns>
        public async Task LockAsync(Func<Task> worker)
        {
            await LockAsync<int>(async () =>
            {
                await worker();
                return 0;
            });
        }

        /// <summary>
        /// Executes the given <see cref="Task{T}"/> on only a single thread at a time, and returns the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="worker"></param>
        /// <returns></returns>
        public async Task<T> LockAsync<T>(Func<Task<T>> worker)
        {
            var isTaken = false;

            try
            {
                do
                {
                    try
                    {
                    }
                    finally
                    {
                        //we use finally because it cannot be interrupted by ThreadAbortException, so isTaken is always correctly updated.
                        //we interrupt the WaitAsync periodically so the calling thread does not get infinitely locked.
                        isTaken = await _semaphore.WaitAsync(TimeSpan.FromSeconds(1));
                    }
                }
                while (!isTaken);

                return await worker();
            }
            finally
            {
                if (isTaken)
                {
                    _semaphore.Release();
                }
            }
        }
    }
}
