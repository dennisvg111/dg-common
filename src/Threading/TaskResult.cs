namespace DG.Common.Threading
{
    /// <summary>
    /// Represents the result of an asynchronous operation, including a value indicating if this operation was successful or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskResult<T>
    {
        private readonly bool _isSuccessFul;
        private readonly T _result;

        /// <summary>
        /// Indicates if the asynchronous operation was successful.
        /// </summary>
        public bool IsSuccessful => _isSuccessFul;

        /// <summary>
        /// Initializes a new instance of <see cref="TaskResult{T}"/> with the given 
        /// </summary>
        /// <param name="isSuccessFul"></param>
        /// <param name="result"></param>
        public TaskResult(bool isSuccessFul, T result)
        {
            _isSuccessFul = isSuccessFul;
            _result = result;
        }

        /// <summary>
        /// Tries to retrieve the result of the operation, and returns a value indicating if this was successful.
        /// </summary>
        /// <param name="result">The result of the operation</param>
        /// <returns>A value indicating if the asynchronous operation was succesful</returns>
        public bool TryGet(out T result)
        {
            if (_isSuccessFul)
            {
                result = _result;
                return true;
            }
            result = default(T);
            return false;
        }
    }

    /// <summary>
    /// Provides initialization methods for <see cref="TaskResult{T}"/>.
    /// </summary>
    public static class TaskResult
    {
        /// <summary>
        /// Returns a new instance of <see cref="TaskResult{T}"/> with the given <paramref name="result"/>, and <see cref="TaskResult{T}.IsSuccessful"/> set to <see langword="true"/>.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static TaskResult<T> Success<T>(T result)
        {
            return new TaskResult<T>(true, result);
        }

        /// <summary>
        /// Returns a new instance of <see cref="TaskResult{T}"/> where the result is set to the default value of <typeparamref name="T"/>, and <see cref="TaskResult{T}.IsSuccessful"/> set to <see langword="false"/>.
        /// </summary>
        /// <returns></returns>
        public static TaskResult<T> Failure<T>()
        {
            return new TaskResult<T>(false, default(T));
        }
    }
}
