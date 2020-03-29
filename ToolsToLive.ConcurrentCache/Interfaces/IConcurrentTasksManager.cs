using System;
using System.Threading.Tasks;

namespace ToolsToLive.ConcurrentCache.Interfaces
{
    public interface IConcurrentTasksManager
    {
        Task<T> RunTask<T>(string key, Func<T> dataSourceFunc);
        Task<T> RunTask<T>(string key, Func<Task<T>> dataSourceFunc);
    }
}
