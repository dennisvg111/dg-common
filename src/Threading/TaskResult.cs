namespace DG.Common.Threading
{
    /// <summary>
    /// Represents the result of an asynchronous operation, including a value indicating if this operation was successful or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskResult<T>
    {
        private readonly TaskResultType _type;
        private readonly T _result;

        /// <summary>
        /// Indicates if the asynchronous operation was successful.
        /// </summary>
        public bool IsSuccessful => _type == TaskResultType.Success;

        /// <summary>
        /// Indicates if an exception that differs from an expected failure has occured during the operation.
        /// </summary>
        public bool FailedBecauseOfException => _type == TaskResultType.UnexpectedException;

        /// <summary>
        /// Initializes a new instance of <see cref="TaskResult{T}"/> with the given <paramref name="result"/>.
        /// </summary>
        /// <param name="isSuccessFul"></param>
        /// <param name="result"></param>
        internal TaskResult(TaskResultType isSuccessFul, T result)
        {
            _type = isSuccessFul;
            _result = result;
        }

        /// <summary>
        /// Tries to retrieve the result of the operation, and returns a value indicating if this was successful.
        /// </summary>
        /// <param name="result">The result of the operation</param>
        /// <returns>A value indicating if the asynchronous operation was succesful</returns>
        public bool TryGet(out T result)
        {
            if (IsSuccessful)
            {
                result = _result;
                return true;
            }
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Provides initialization methods for <see cref="TaskResult{T}"/>.
    /// </summary>
    public static class TaskResult
    {
        /// <summary>
        /// <para>Returns a new instance of <see cref="TaskResult{T}"/> representing a successful result.</para>
        /// <para>The result of the operation is set to <paramref name="result"/>, and <see cref="TaskResult{T}.IsSuccessful"/> is set to <see langword="true"/></para>
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static TaskResult<T> Success<T>(T result)
        {
            return new TaskResult<T>(TaskResultType.Success, result);
        }

        /// <summary>
        /// <para>Returns a new instance of <see cref="TaskResult{T}"/> representing an expected type of failure.</para>
        /// <para>The result of the operation is set to the default value of <typeparamref name="T"/>, and <see cref="TaskResult{T}.IsSuccessful"/> is set to <see langword="false"/></para>
        /// </summary>
        /// <returns></returns>
        public static TaskResult<T> Failure<T>()
        {
            return new TaskResult<T>(TaskResultType.Failure, default);
        }

        /// <summary>
        /// <para>Returns a new instance of <see cref="TaskResult{T}"/> representing an unexpected exception.</para>
        /// <para>The result of the operation is set to the default value of <typeparamref name="T"/>, and <see cref="TaskResult{T}.IsSuccessful"/> is set to <see langword="false"/></para>
        /// </summary>
        /// <returns></returns>
        public static TaskResult<T> UnexpectedException<T>()
        {
            return new TaskResult<T>(TaskResultType.UnexpectedException, default);
        }
    }

    internal enum TaskResultType
    {
        Success,
        Failure,
        UnexpectedException
    }
}
