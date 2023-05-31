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

        public async Task LockAsync(Func<Task> worker)
        {
            await LockAsync<int>(async () =>
            {
                await worker();
                return 0;
            });
        }

        // overloading variant for non-void methods with return type (generic T)
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
