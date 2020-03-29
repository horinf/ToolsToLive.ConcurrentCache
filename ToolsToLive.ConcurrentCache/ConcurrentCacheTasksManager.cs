using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolsToLive.ConcurrentCache.Interfaces;

namespace ToolsToLive.ConcurrentCache
{
    public class ConcurrentCacheTasksManager
    {
        private readonly IConcurrentTasksManager _concurrentTasksManager;

        public ConcurrentCacheTasksManager(
            IConcurrentTasksManager concurrentTasksManager)
        {
            _concurrentTasksManager = concurrentTasksManager;
        }

        internal Task<T> RegisterRequestAndReturnTask<T>(string key, Func<T> dataSourceFunc, ICacheStorage cacheStorage, TimeSpan expirationTimeSpan)
        {
            var task = _concurrentTasksManager.RunTask(key, dataSourceFunc);
            AttachTask(task, key, cacheStorage, expirationTimeSpan);
            return task;
        }

        internal Task<T> RegisterRequestAndReturnTask<T>(string key, Func<Task<T>> dataSourceFunc, ICacheStorage cacheStorage, TimeSpan expirationTimeSpan)
        {
            var task = _concurrentTasksManager.RunTask(key, dataSourceFunc);
            AttachTask(task, key, cacheStorage, expirationTimeSpan);
            return task;
        }

        /// <summary>
        /// Task to add the resulting value to the cache and remove the task from the dictionary.
        /// Don't wait for the value to be written to the cache - the result is immediately returned,
        /// and then in the same background thread the value is stored in the cache without haste.
        /// </summary>
        private static void AttachTask<T>(Task<T> task, string key, ICacheStorage cacheStorage, TimeSpan expirationTimeSpan)
        {
            task.ContinueWith(async (t) =>
            {
                try
                {
                    // Cache storage can be a custom class, so we need to make sure it is safe for this code.
                    // If something went wrong - do nothing. Exception handling is the responsibility of the method that was called to get the value.
                    if (!t.IsFaulted && t.IsCompleted)
                    {
                        await cacheStorage.Set(key, t.Result, expirationTimeSpan);
                    }
                }
                catch (ThreadAbortException) { } //The calling thread may have already closed and no longer needs the cache.
                finally
                {
                }
            });
        }
    }
}
