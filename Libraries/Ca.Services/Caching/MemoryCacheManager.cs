using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Ca.Services.Caching
{
    public class MemoryCacheManager : ICacheManager
    {
        private readonly int slidingExpirationSec, absoluteExpirationSec;

        private readonly IMemoryCache _memoryCache;

        public MemoryCacheManager(IMemoryCache memoryCache, IOptions<CacheOptions> options)
        {
            _memoryCache = memoryCache;
            slidingExpirationSec = options.Value.SlidingExpirationSec;
            absoluteExpirationSec = options.Value.AbsoluteExpirationSec;
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

        public void Delete(string key)
        {
            _memoryCache.Remove(key);
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