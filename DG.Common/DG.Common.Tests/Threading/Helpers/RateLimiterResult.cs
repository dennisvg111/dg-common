using System;
using System.Threading.Tasks;

namespace DG.Common.Tests.Threading.Helpers
{
    public class RateLimiterResult
    {
        private readonly DateTime _taskStartTime = DateTime.Now;
        private DateTime _actualStartTime;

        public DateTime TaskStartTime => _taskStartTime;
        public TimeSpan RateLimitedFor => _actualStartTime - _taskStartTime;

        public async Task SetActualStartTime()
        {
            _actualStartTime = DateTime.Now;

            //wait so we can test if the rate limiter takes task time into account.
            await Task.Delay(1000);
        }

        public override string ToString()
        {
            return RateLimitedFor.ToString();
        }
    }
}
