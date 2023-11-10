using System;
using System.Threading;
using System.Threading.Tasks;

namespace DG.Common.Threading
{
    /// <summary>
    /// Provides functionality for safely calling asynchronous functions from an environment that doesn't support running code asynchronously.
    /// </summary>
    public static class SafeSync
    {
        private static readonly TaskFactory _myTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        /// <summary>
        /// Runs the given asynchronous <paramref name="func"/> synchronously, and immediately returns the <typeparamref name="TResult"/> result.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TResult Run<TResult>(Func<Task<TResult>> func)
        {
            return _myTaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs the given asynchronous <paramref name="func"/> synchronously.
        /// </summary>
        /// <param name="func"></param>
        public static void Run(Func<Task> func)
        {
            _myTaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }
    }
}
