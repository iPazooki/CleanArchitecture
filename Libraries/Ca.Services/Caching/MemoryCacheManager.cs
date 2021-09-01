using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Ca.Services.Caching
{
    public class MemoryCacheManager : ICacheManager
    {
        private const int slidingExpirationSec = 10, absoluteExpirationSec = 60;

        private readonly IMemoryCache _memoryCache;

        public MemoryCacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<TEntity> Get<TEntity>(string key, Func<string, Task<TEntity>> acquire)
        {
            if (_memoryCache.TryGetValue(key, out TEntity result))
                return result;

            result = await acquire(key);

            if (result != null)
                Set(key, result);

            return result;
        }

        private void Set<TEntity>(string key, TEntity result)
        {
            var option = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromSeconds(slidingExpirationSec),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(absoluteExpirationSec)
            };

            _memoryCache.Set(key, result, option);
        }
    }
}