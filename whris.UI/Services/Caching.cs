using Microsoft.Extensions.Caching.Memory;

namespace whris.UI.Services
{
    public class Caching
    {
        public static  MemoryCacheEntryOptions cacheEntryOptions => new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1))
            .SetPriority(CacheItemPriority.Normal);

        public static string EmployeeCmbDsCacheKey => "EmployeeCmbDs";
    }
}
