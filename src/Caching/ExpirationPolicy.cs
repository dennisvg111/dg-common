using System;

namespace DG.Common.Caching
{
    /// <summary>
    /// Defines the expiration policy for a caching mechanism.
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
                if (_specificTimeOfDay.HasValue)
                {
                    return ExpirationType.TimeOfDay;
                }
                return ExpirationType.None;
            }
        }

        private ExpirationPolicy(TimeSpan? slidingExpiration, TimeSpan? maxSlidingExpiration, TimeSpan? specificTimeOfDay)
        {
            _slidingExpiration = slidingExpiration;
            _maxSlidingExpiration = maxSlidingExpiration;
            _specificTimeOfDay = specificTimeOfDay;
        }

        /// <summary>
        /// Defines a method that creates a <typeparamref name="T"/> based on a given <paramref name="slidingExpiration"/> and <paramref name="absoluteExpiration"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="slidingExpiration"></param>
        /// <param name="absoluteExpiration"></param>
        /// <returns></returns>
        public delegate T PolicyConverter<T>(TimeSpan? slidingExpiration, DateTimeOffset? absoluteExpiration);

        /// <summary>
        /// Converts this <see cref="ExpirationPolicy"/> to an instance of <typeparamref name="T"/>, using the given <paramref name="policyConverter"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="policyConverter"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public T ConvertTo<T>(PolicyConverter<T> policyConverter)
        {
            var expirationType = Type;
            if (expirationType == ExpirationType.None)
            {
                return policyConverter(null, null);
            }

            switch (expirationType)
            {
                case ExpirationType.Sliding:
                    return policyConverter(_slidingExpiration.Value, null);
                case ExpirationType.Absolute:
                    return policyConverter(null, DateTimeOffset.UtcNow + _maxSlidingExpiration.Value);
                case ExpirationType.TimeOfDay:
                    var currentDate = DateTime.Now;
                    var nextTimeHit = currentDate.Date + _specificTimeOfDay.Value;
                    if (nextTimeHit < currentDate)
                    {
                        nextTimeHit = nextTimeHit.AddDays(1);
                    }
                    return policyConverter(null, DateTimeOffset.UtcNow + (nextTimeHit - currentDate));
                default:
                    throw new NotImplementedException($"Function {nameof(ConvertTo)} has not been implemented for policy type {Type}.");
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
            TimeOfDay,
            /// <summary>
            /// Values should not be evicted.
            /// </summary>
            None
        }
    }
}
