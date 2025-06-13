using Microsoft.Extensions.Caching.Memory;
using System;

namespace DG.Common.Caching
{
    /// <summary>
    /// Defines the expiration policy for a <see cref="TypedCache{T}"/>.
    /// </summary>
    public sealed class ExpirationPolicy
    {
        private readonly TimeSpan? _slidingExpiration;
        private readonly TimeSpan? _maxSlidingExpiration;
        private readonly TimeSpan? _specificTimeOfDay;

        /// <summary>
        /// The type of expiration used.
        /// </summary>
        public ExpirationType Type
        {
            get
            {
                if (_slidingExpiration.HasValue)
                {
                    return ExpirationType.Sliding;
                }
                if (_maxSlidingExpiration.HasValue)
                {
                    return ExpirationType.Absolute;
                }
                return ExpirationType.TimeOfDay;
            }
        }

        private ExpirationPolicy(TimeSpan? slidingExpiration, TimeSpan? maxSlidingExpiration, TimeSpan? specificTimeOfDay)
        {
            _slidingExpiration = slidingExpiration;
            _maxSlidingExpiration = maxSlidingExpiration;
            _specificTimeOfDay = specificTimeOfDay;
        }

        /// <summary>
        /// Returns <see cref="MemoryCacheEntryOptions"/> as defined by this expiration.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public MemoryCacheEntryOptions GetCacheEntryOptions()
        {
            switch (Type)
            {
                case ExpirationType.Sliding:
                    return new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = _slidingExpiration.Value
                    };
                case ExpirationType.Absolute:
                    return new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpiration = DateTimeOffset.UtcNow + _maxSlidingExpiration.Value
                    };
                case ExpirationType.TimeOfDay:
                    var currentDate = DateTime.Now;
                    var nextTimeHit = currentDate.Date + _specificTimeOfDay.Value;
                    if (nextTimeHit < currentDate)
                    {
                        nextTimeHit = nextTimeHit.AddDays(1);
                    }
                    return new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpiration = DateTimeOffset.Now + (nextTimeHit - currentDate)
                    };
                default:
                    throw new NotImplementedException($"Function {nameof(GetCacheEntryOptions)} has not been implemented for type {Type}.");
            }
        }

        /// <summary>
        /// Returns a new instance of <see cref="ExpirationPolicy"/> where values should be evicted if they have not been accessed in a given span of time.
        /// </summary>
        /// <param name="slidingExpiration"></param>
        /// <returns></returns>
        public static ExpirationPolicy ForSlidingExpiration(TimeSpan slidingExpiration)
        {
            return new ExpirationPolicy(slidingExpiration, null, null);
        }

        /// <summary>
        /// Returns a new instance of <see cref="ExpirationPolicy"/> where values should be evicted after a given amount of time.
        /// </summary>
        /// <param name="absoluteExpiration"></param>
        /// <returns></returns>
        public static ExpirationPolicy ForAbsoluteExpiration(TimeSpan absoluteExpiration)
        {
            return new ExpirationPolicy(null, absoluteExpiration, null);
        }

        /// <summary>
        /// Returns a new instance of <see cref="ExpirationPolicy"/> where values should be evicted on a specific time of day.
        /// </summary>
        /// <param name="timeOfDay"></param>
        /// <returns></returns>
        public static ExpirationPolicy ForTimeOfDay(TimeSpan timeOfDay)
        {
            return new ExpirationPolicy(null, null, timeOfDay);
        }

        /// <summary>
        /// This enum indicates in what manner it should be determined if values need to be evicted.
        /// </summary>
        public enum ExpirationType
        {
            /// <summary>
            /// Values should be evicted if they have not been accessed in a given span of time.
            /// </summary>
            Sliding,
            /// <summary>
            /// Values should be evicted after a given amount of time.
            /// </summary>
            Absolute,
            /// <summary>
            /// Values should be evicted on a specific time of day.
            /// </summary>
            TimeOfDay
        }
    }
}
