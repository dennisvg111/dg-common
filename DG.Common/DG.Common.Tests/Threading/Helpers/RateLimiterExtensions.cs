using DG.Common.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DG.Common.Tests.Threading.Helpers
{
    public static class RateLimiterExtensions
    {
        public static async Task<RateLimiterResult[]> ExecuteNFunctionsAsync(this RateLimiter limiter, int amountOfRequests)
        {
            List<Task> tasks = new List<Task>();
            RateLimiterResult[] results = new RateLimiterResult[amountOfRequests];

            for (int i = 0; i < amountOfRequests; i++)
            {
                var result = new RateLimiterResult();
                results[i] = result;
                tasks.Add(limiter.ExecuteAsync(() => result.SetActualStartTime()));
            }

            await Task.WhenAll(tasks);
            return results;
        }
    }
}
