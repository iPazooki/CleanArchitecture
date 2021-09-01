using System;
using System.Threading.Tasks;

namespace Ca.Services.Caching
{
    /// <summary>
    /// Represents a cache management
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="TEntity">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<TEntity> Get<TEntity>(string key, Func<string, Task<TEntity>> acquire);

        void Delete(string key);
    }
}