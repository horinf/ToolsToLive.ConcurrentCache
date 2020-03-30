using System;
using System.Threading.Tasks;
using ToolsToLive.ConcurrentCache.Interfaces;

namespace ToolsToLive.ConcurrentCache
{
    /// <summary>
    /// Concurrent cache.
    /// When retrieving a cache (calling the “Get()” or “GetAsync()” method), an attempt is made to retrieve a value from the cache storage
    /// (using the storage interface class passed in the constructor - use dependency injection to customize it).
    /// In case of successful retrieval of the value from the cache, it is immediately returned to the calling code.
    /// If there is no value in the cache, it is retrieved using the function passed in the parameter,
    /// returned to the calling code, and stored in the cache.
    /// Only one task to retrieve a value is running for each key, regardless of the number of concurrent requests from different threads. In this case,
    /// all threads will wait for the task to be completed and, after its completion, will receive the corresponding value.
    /// </summary>
    public class ConcurrentCache : IConcurrentCache
    {
        private readonly ICacheStorage _cacheStorage;
        private readonly IConcurrentCacheTasksManager _concurrentCacheTasksManager;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cacheStore">Cache storage interface, e.g. memory cache, Redis or combined cache.</param>
        public ConcurrentCache(ICacheStorage cacheStore,
            IConcurrentCacheTasksManager concurrentCacheTasksManager)
        {
            _cacheStorage = cacheStore;
            _concurrentCacheTasksManager = concurrentCacheTasksManager;
        }

        #region depricated
        ///// <inheritdoc />
        //public T Get<T>(string key)
        //{
        //    T cacheData = _cacheStorage.Get<T>(key).Result;

        //    if (cacheData == null)
        //    {
        //        return default;
        //    }

        //    return cacheData;
        //}

        ///// <inheritdoc />
        //public T Get<T>(string key, Func<T> dataSourceFunc, TimeSpan expirationTimeSpan)
        //{
        //    T cacheData = _cacheStorage.Get<T>(key).Result;

        //    if (cacheData != null)
        //    {
        //        return cacheData;
        //    }

        //    Task<T> task = TasksWorker.RegisterRequestAndReturnTask(key, dataSourceFunc, _cacheStorage, expirationTimeSpan);
        //    return task.Result;
        //}
        #endregion

        /// <inheritdoc />
        public async Task<T> GetAsync<T>(string key, Func<T> dataSourceFunc, TimeSpan expirationTimeSpan)
        {
            T cacheData = await _cacheStorage.Get<T>(key);
            if (cacheData != null)
            {
                return cacheData;
            }

            return await _concurrentCacheTasksManager.RegisterRequestAndReturnTask(key, dataSourceFunc, _cacheStorage, expirationTimeSpan).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataSourceFunc, TimeSpan expirationTimeSpan)
        {
            T cacheData = await _cacheStorage.Get<T>(key);
            if (cacheData != null)
            {
                return cacheData;
            }

            return await _concurrentCacheTasksManager.RegisterRequestAndReturnTask(key, dataSourceFunc, _cacheStorage, expirationTimeSpan).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task WarmUp<T>(string key, T value, TimeSpan expirationTimeSpan)
        {
            await _cacheStorage.Set<T>(key, value, expirationTimeSpan).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task WarmUp<T>(string key, Func<Task<T>> dataSourceFunc, TimeSpan expirationTimeSpan)
        {
            await _concurrentCacheTasksManager.RegisterRequestAndReturnTask(key, dataSourceFunc, _cacheStorage, expirationTimeSpan).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task Remove(string key)
        {
            await _cacheStorage.Remove(key).ConfigureAwait(false);
        }
    }
}
