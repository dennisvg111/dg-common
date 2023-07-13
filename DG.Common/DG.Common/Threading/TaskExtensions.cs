using System.Threading.Tasks;

namespace DG.Common.Threading
{
    /// <summary>
    /// Provides extension methods for <see cref="Task{TResult}"/>.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// <para>Gets the result value of this <see cref="Task{TResult}"/>, unwrapping exceptions if needed.</para>
        /// <para>Note that this is functionally the same as retrieving the result of <see cref="Task.GetAwaiter()"/></para>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns>The result value of this <see cref="Task{TResult}"/>, which is the same as the task's type parameter.</returns>
        public static T GetUnwrappedResult<T>(this Task<T> task)
        {
            //use Task.Run to avoid deadlocks, if the task would execute on the current thread.
            return Task.Run(async () => await task).GetAwaiter().GetResult();
        }
    }
}
