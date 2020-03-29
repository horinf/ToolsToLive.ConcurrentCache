using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ToolsToLive.ConcurrentCache.Interfaces;

namespace ToolsToLive.ConcurrentCache
{
    public class ConcurrentTasksManager : IConcurrentTasksManager
    {
        /// requests to retreive value (e.g. for cache). No reason to use cuncurrentDictionary because we need to manage requests anyway.
        private static readonly Dictionary<string, Task> _requestsList;
        private static readonly object _requestLocker;

        static ConcurrentTasksManager()
        {
            _requestsList = new Dictionary<string, Task>();
            _requestLocker = new object();
        }

        /// <summary>
        /// Register a task to get a value from data source function (save task to list).
        /// If the task already exists, it is not created (returned from the list).
        /// </summary>
        public Task<T> RunTask<T>(string key, Func<T> dataSourceFunc)
        {
            Task<T> task;
            lock (_requestLocker)
            {
                if (_requestsList.ContainsKey(key))
                {
                    return (Task<T>)_requestsList[key];
                }

                task = CreateTaskForGetValueFromFuncAndUpdateCache(key, dataSourceFunc);
                _requestsList.Add(key, task);
            }

            return task;
        }

        /// <summary>
        /// Register a task to get a value from data source function (save task to list).
        /// If the task already exists, it is not created (returned from the list).
        /// </summary>
        public Task<T> RunTask<T>(string key, Func<Task<T>> dataSourceFunc)
        {
            Task<T> task;
            lock (_requestLocker)
            {
                if (_requestsList.ContainsKey(key))
                {
                    return (Task<T>)_requestsList[key];
                }

                task = CreateTaskForGetValueFromFuncAndUpdateCache(key, dataSourceFunc);
                _requestsList.Add(key, task);
            }

            return task;
        }

        /// <summary>
        /// Create task to get a value from data source function and remove a task from the list after it is completed.
        /// </summary>
        private Task<T> CreateTaskForGetValueFromFuncAndUpdateCache<T>(string key, Func<T> dataSourceFunc)
        {
            Task<T> task = Task.Run(dataSourceFunc);
            AttachTask(task, key);
            return task;
        }

        /// <summary>
        /// Create task to get a value from data source function and remove a task from the list after it is completed.
        /// </summary>
        private Task<T> CreateTaskForGetValueFromFuncAndUpdateCache<T>(string key, Func<Task<T>> dataSourceFunc)
        {
            Task<T> task = dataSourceFunc.Invoke();
            AttachTask(task, key);
            return task;
        }

        /// <summary>
        /// Task to remove the task from the dictionary.
        /// </summary>
        private void AttachTask<T>(Task<T> task, string key)
        {
            task.ContinueWith((t) =>
            {
                try
                {
                    // If something went wrong - do nothing. Exception handling is the responsibility of the method that was called to get the value.
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
