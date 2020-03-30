using System;
using System.Threading.Tasks;
using ToolsToLive.ConcurrentCache.Model;

namespace ToolsToLive.ConcurrentCache.Interfaces
{
    /// <summary>
    /// Cache storage interface, e.g. memory cache, Redis or a combination of different ones.
    /// </summary>
    public interface ICacheStorage
    {
        /// <summary>
        /// Adds to storage. Existing value for key will be replaced.
        /// </summary>
        /// <typeparam name="T">Type of cached value.</typeparam>
        /// <param name="key">Key to store.</param>
        /// <param name="value">Value associated to key.</param>
        /// <param name="expirationTimeSpan">Cache life time.</param>
        Task Set<T>(string key, T value, TimeSpan expirationTimeSpan);

        /// <summary>
        /// Retrieves item from the cache by key.
        /// </summary>
        /// <typeparam name="T">Type of cached value.</typeparam>
        /// <param name="key">The key for the cache item to retrieve.</param>
        /// <returns>The retrieved cache item, or null if the value is not found by passed key.</returns>
        Task<CacheData<T>> Get<T>(string key);

        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key">A key for the cache item to remove.</param>
        Task Remove(string key);
    }
}
