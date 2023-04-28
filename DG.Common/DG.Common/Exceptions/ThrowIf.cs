namespace DG.Common.Exceptions
{
    /// <summary>
    /// Provides methods that throw exceptions on specific conditions, to reduce boilerplate code.
    /// </summary>
    public static class ThrowIf
    {

        /// <summary>
        /// Provides methods to safeguard function parameters.
        /// </summary>
        public static ParameterChecks Parameter { get; } = new ParameterChecks();
    }
}
