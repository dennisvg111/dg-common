using System.Collections.Concurrent;

namespace DG.Common.Locking
{
    /// <summary>
    /// This class provides functionality to create and reuse objects to lock on, threadsafe.
    /// </summary>
    public sealed class LockProvider
    {
        private readonly object _defaultLock;
        private readonly ConcurrentDictionary<long, object> _longLocks;
        private readonly ConcurrentDictionary<string, object> _stringLocks;

        /// <summary>
        /// Creates a new instance of a <see cref="LockProvider"/>.
        /// </summary>
        public LockProvider()
        {
            _defaultLock = new object();
            _longLocks = new ConcurrentDictionary<long, object>();
            _stringLocks = new ConcurrentDictionary<string, object>();
        }

        /// <summary>
        /// Returns an object to lock on. Note that this object is shared for this instance of <see cref="LockProvider"/>.
        /// </summary>
        public object DefaultLock => _defaultLock;

        /// <summary>
        /// Returns an object to lock on for the given <see cref="long"/> index. Other calls with the same index for the same instance of <see cref="LockProvider"/> will return the same object.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[long index]
        {
            get
            {
                return _longLocks.GetOrAdd(index, (i) => new object());
            }
        }

        /// <summary>
        /// Returns an object to lock on for the given <see cref="string"/> index. Other calls with the same index for the same instance of <see cref="LockProvider"/> will return the same object.
        /// </summary>
        public object this[string index]
        {
            get
            {
                return _stringLocks.GetOrAdd(index, (i) => new object());
            }
        }

        /// <summary>
        /// <inheritdoc cref="this[long]"/>
        /// <para></para>
        /// Note that calling this method is functionally the same as using the <see cref="this[long]"/> indexer.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetLock(long index)
        {
            return this[index];
        }

        /// <summary>
        /// <inheritdoc cref="this[string]"/>
        /// <para></para>
        /// Note that calling this method is functionally the same as using the <see cref="this[string]"/> indexer.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetLock(string index)
        {
            return this[index];
        }

        #region Instance managing logic
        private static readonly LockProvider _defaultInstance = new LockProvider();
        private static readonly ConcurrentDictionary<string, LockProvider> _namedInstances = new ConcurrentDictionary<string, LockProvider>();

        /// <summary>
        /// Returns the default shared <see cref="LockProvider"/> instance.
        /// </summary>
        public static LockProvider Default => _defaultInstance;

        /// <summary>
        /// Returns a shared instance of the <see cref="LockProvider"/> by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static LockProvider ByName(string name)
        {
            return _namedInstances.GetOrAdd(name, (n) => new LockProvider());
        }
        #endregion
    }
}
