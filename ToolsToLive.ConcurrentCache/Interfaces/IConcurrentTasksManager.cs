using System;
using System.Threading.Tasks;

namespace ToolsToLive.ConcurrentCache.Interfaces
{
    public interface IConcurrentTasksManager
    {
        /// <summary>
        /// Register a task to get a value from data source function (save task to list).
        /// If the task already exists, it is not created (returned from the list).
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">
        /// Unique key for task.
        /// If second task with the same key comes when first task is still being executed, task won't be executed. First task will be returned.
        /// </param>
        /// <param name="dataSourceFunc">Task to run.</param>
        /// <returns>Task that was passed in the params or task from list, if task with the same key already exists.</returns>
        Task<T> RunTask<T>(string key, Func<T> dataSourceFunc);

        /// <summary>
        /// Register a task to get a value from data source function (save task to list).
        /// If the task already exists, it is not created (returned from the list).
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">
        /// Unique key for task.
        /// If second task with the same key comes when first task is still being executed, task won't be executed. First task will be returned.
        /// </param>
        /// <param name="dataSourceFunc">Task to run.</param>
        /// <returns>Task that was passed in the params or task from list, if task with the same key already exists.</returns>
        Task<T> RunTask<T>(string key, Func<Task<T>> dataSourceFunc);
    }
}
