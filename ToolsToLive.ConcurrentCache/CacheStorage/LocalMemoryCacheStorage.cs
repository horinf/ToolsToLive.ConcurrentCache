using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using ToolsToLive.ConcurrentCache.Interfaces;

namespace ToolsToLive.ConcurrentCache.CacheStorage
{
    /// <summary>
    /// Simple wrapper around MemoryCache with absolute expiration only.
    /// </summary>
    public class LocalMemoryCacheStorage : ICacheStorage
    {
        /// <inheritdoc />
        public Task Set<T>(string key, T value, TimeSpan expirationTimeSpan)
        {
            if (expirationTimeSpan == TimeSpan.Zero)
            {
                throw new ArgumentException(nameof(expirationTimeSpan));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key for cache can not be null or empty");
            }

            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow + expirationTimeSpan
            };

            var valueCacheItem = new CacheItem(key, value);
            MemoryCache.Default.Set(valueCacheItem, policy);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<T> Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key for cache can not be null or empty");
            }

            object result = MemoryCache.Default.GetCacheItem(key)?.Value;

            if (object.Equals(result, null))
            {
                return null;
            }

            return Task.FromResult((T)result);
        }

        /// <inheritdoc />
        public Task Remove(string key)
        {
            MemoryCache.Default.Remove(key);
            return Task.CompletedTask;
        }
    }
}
