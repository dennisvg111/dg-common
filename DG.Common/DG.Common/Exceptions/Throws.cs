namespace DG.Common.Exceptions
{
    /// <summary>
    /// Provides methods that throw exceptions on specific conditions, to reduce boilerplate code.
    /// </summary>
    public class Throws
    {
        private Throws()
        {

        }

        /// <summary>
        /// The entry point for the <see cref="Throws"/> class.
        /// </summary>
        public static Throws If => new Throws();
    }
}
