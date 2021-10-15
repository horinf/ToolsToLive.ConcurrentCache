using System;
using System.Threading.Tasks;

namespace ToolsToLive.ConcurrentCache.Interfaces
{
    /// <summary>
    /// Concurrent cache.
    /// </summary>
    public interface IConcurrentCache
    {
        #region depricated
        // Only async methods is allows in order to avoid deadlocks. 
        // If it is necessary to use sync methods -- it is better to use something to run async methods in sync code. (Can be encapsulated here, but later).

        ///// <summary>
        ///// Gets value of key.
        ///// </summary>
        ///// <param name="key">A key for the cache item to retreive.</param>
        //T Get<T>(string key);

        ///// <summary>
        ///// Gets value of key. If caches are empty, it fetches (and returns) value from <paramref name="dataSourceFunc"/> and populate the cache.
        ///// For each key, only one function is executed at a time. Other requests wait for this function and get its result.
        ///// </summary>
        ///// <typeparam name="T">Type of value.</typeparam>
        ///// <param name="key">A key for the cache item to retreive.</param>
        ///// <param name="dataSourceFunc">Data source function.</param>
        ///// <param name="expirationTimeSpan">Cache life time.</param>
        //T Get<T>(string key, Func<T> dataSourceFunc, TimeSpan expirationTimeSpan);
        #endregion

        /// <summary>
        /// Gets value of key. If caches are empty, it will fetch from <paramref name="dataSourceFunc"/> and populate the cache.
        /// For each key, only one function is executed at a time. Other requests wait for this function and get its result.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="key">A key for the cache item to retreive.</param>
        /// <param name="dataSourceFunc">Data source function</param>
        /// <param name="expirationTimeSpan">Cache life time.</param>
        Task<T> GetAsync<T>(string key, Func<T> dataSourceFunc, TimeSpan expirationTimeSpan);

        /// <summary>
        /// Gets value of key. If caches are empty, it will fetch from <paramref name="dataSourceFunc"/> and populate the cache.
        /// For each key, only one function is executed at a time. Other requests wait for this function and get its result.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="key">A key for the cache item to retreive.</param>
        /// <param name="dataSourceFunc">Data source function</param>
        /// <param name="expirationTimeSpan">Cache life time.</param>
        Task<T> GetAsync<T>(string key, Func<Task<T>> dataSourceFunc, TimeSpan expirationTimeSpan);

        /// <summary>
        /// Gets value of key. If caches are empty, returns default value of T.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="key">A key for the cache item to retreive.</param>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Adds the specified item to the cache. Usually it is better to use Get method with passed Func to retreive value.
        /// Try to avoid using this method, because it will lose all the advantages of competitive access. Use the Get method with "dataSourceFunc" and "expirationTimeSpan" instead.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="key">A key for the cache item to store.</param>
        /// <param name="value">Value to store.</param>
        /// <param name="expirationTimeSpan">Cache life time.</param>
        Task WarmUp<T>(string key, T value, TimeSpan expirationTimeSpan);

        /// <summary>
        /// Adds the specified item to the cache. Usually it is better to use Get method with passed Func to retreive value.
        /// For each key, only one function is executed at a time. Other requests wait for this function and get its result.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="key">A key for the cache item to store.</param>
        /// <param name="value">Value to store.</param>
        /// <param name="expirationTimeSpan">Cache life time.</param>
        Task WarmUp<T>(string key, Func<Task<T>> dataSourceFunc, TimeSpan expirationTimeSpan);

        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key">A key for the cache item to remove.</param>
        Task Remove(string key);
    }
}
