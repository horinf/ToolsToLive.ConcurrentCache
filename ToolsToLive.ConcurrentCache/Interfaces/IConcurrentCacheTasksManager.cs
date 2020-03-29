using System;
using System.Threading.Tasks;

namespace ToolsToLive.ConcurrentCache.Interfaces
{
    public interface IConcurrentCacheTasksManager
    {
        Task<T> RegisterRequestAndReturnTask<T>(string key, Func<T> dataSourceFunc, ICacheStorage cacheStorage, TimeSpan expirationTimeSpan);
        Task<T> RegisterRequestAndReturnTask<T>(string key, Func<Task<T>> dataSourceFunc, ICacheStorage cacheStorage, TimeSpan expirationTimeSpan);
    }
}
