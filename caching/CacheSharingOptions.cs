namespace DG.Common.Caching
{
    /// <summary>
    /// Defines the options for cache sharing.
    /// </summary>
    public enum CacheSharingOptions
    {
        /// <summary>
        /// Share cache items with other instances using the same type.
        /// </summary>
        Shared,
        /// <summary>
        /// Do not share cache items with other instances.
        /// </summary>
        Unique
    }
}
