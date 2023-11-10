using System;
using System.Threading;
using System.Threading.Tasks;

namespace DG.Common.Threading
{
    /// <summary>
    /// Provides functionality for safely executing asynchronous operations from an environment that only supports running code synchronously.
    /// </summary>
    public static class SafeSync
    {
        private static readonly TaskFactory _factory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        /// <summary>
        /// Runs the given asynchronous <paramref name="asyncOperation"/> synchronously, and immediately returns the <typeparamref name="TResult"/> result.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="asyncOperation"></param>
        /// <returns></returns>
        public static TResult Run<TResult>(Func<Task<TResult>> asyncOperation)
        {
            return _factory.StartNew(asyncOperation).Unwrap().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs the given asynchronous <paramref name="asyncOperation"/> synchronously.
        /// </summary>
        /// <param name="asyncOperation"></param>
        public static void Run(Func<Task> asyncOperation)
        {
            _factory.StartNew(asyncOperation).Unwrap().GetAwaiter().GetResult();
        }
    }
}
