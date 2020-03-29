using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ToolsToLive.ConcurrentCache.Interfaces;

namespace ToolsToLive.ConcurrentCache
{
    internal static class TasksWorker
    {
        /// requests to retreive value (e.g. for cache). No reason to use cuncurrentDictionary because we need to manage requests anyway.
        private static readonly Dictionary<string, Task> _requestsList;
        private static readonly object _requestLocker;

        static TasksWorker()
        {
            _requestsList = new Dictionary<string, Task>();
            _requestLocker = new object();
        }

        /// <summary>
        /// Register a task to get a value from data source function (save task to list).
        /// If the task already exists, it is not created (returned from the list).
        /// </summary>
        internal static Task<T> RegisterRequestAndReturnTask<T>(string key, Func<T> dataSourceFunc, ICacheStorage cacheStorage, TimeSpan expirationTimeSpan)
        {
            Task<T> task;
            lock (_requestLocker)
            {
                if (_requestsList.ContainsKey(key))
                {
                    return (Task<T>)_requestsList[key];
                }

                task = CreateTaskForGetValueFromFuncAndUpdateCache(key, dataSourceFunc, cacheStorage, expirationTimeSpan);
                _requestsList.Add(key, task);
            }

            return task;
        }

        /// <summary>
        /// Register a task to get a value from data source function (save task to list).
        /// If the task already exists, it is not created (returned from the list).
        /// </summary>
        internal static Task<T> RegisterRequestAndReturnTask<T>(string key, Func<Task<T>> dataSourceFunc, ICacheStorage cacheStorage, TimeSpan expirationTimeSpan)
        {
            Task<T> task;
            lock (_requestLocker)
            {
                if (_requestsList.ContainsKey(key))
                {
                    return (Task<T>)_requestsList[key];
                }

                task = CreateTaskForGetValueFromFuncAndUpdateCache(key, dataSourceFunc, cacheStorage, expirationTimeSpan);
                _requestsList.Add(key, task);
            }

            return task;
        }

        /// <summary>
        /// Create task to get a value from data source function, add data to the cache, and remove a task from the list after it is completed.
        /// </summary>
        private static Task<T> CreateTaskForGetValueFromFuncAndUpdateCache<T>(string key, Func<T> dataSourceFunc, ICacheStorage cacheStorage, TimeSpan expirationTimeSpan)
        {
            Task<T> task = Task.Run(dataSourceFunc);
            AttachTask(task, key, cacheStorage, expirationTimeSpan);
            return task;
        }

        /// <summary>
        /// Create task to get a value from data source function, add data to the cache, and remove a task from the list after it is completed.
        /// </summary>
        private static Task<T> CreateTaskForGetValueFromFuncAndUpdateCache<T>(string key, Func<Task<T>> dataSourceFunc, ICacheStorage cacheStorage, TimeSpan expirationTimeSpan)
        {
            Task<T> task = dataSourceFunc.Invoke();
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
            task.ContinueWith((t) =>
            {
                try
                {
                    // Cache storage can be a custom class, so we need to make sure it is safe for this code.
                    // If something went wrong - do nothing. Exception handling is the responsibility of the method that was called to get the value.
                    if (!t.IsFaulted && t.IsCompleted)
                    {
                        cacheStorage.Set(key, t.Result, expirationTimeSpan);
                    }
                }
                catch (ThreadAbortException) { } //The calling thread may have already closed and no longer needs the cache.
                finally
                {
                    lock (_requestLocker)
                    {
                        if (_requestsList.ContainsKey(key))
                        {
                            _requestsList.Remove(key);
                        }
                    }
                }
            });
        }
    }

}
