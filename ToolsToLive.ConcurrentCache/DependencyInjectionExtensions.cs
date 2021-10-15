using Microsoft.Extensions.DependencyInjection;
using ToolsToLive.ConcurrentCache.CacheStorage;
using ToolsToLive.ConcurrentCache.Interfaces;

namespace ToolsToLive.ConcurrentCache
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddLocalMemoryConcurrentCache(this IServiceCollection services)
        {
            services.AddConcurrentCache();
            services.AddSingleton<ICacheStorage, LocalMemoryCacheStorage>();

            return services;
        }

        public static IServiceCollection AddConcurrentCache(this IServiceCollection services)
        {
            services.AddSingleton<IConcurrentCache, ConcurrentCache>();
            services.AddSingleton<IConcurrentCacheTasksManager, ConcurrentCacheTasksManager>();
            services.AddSingleton<IConcurrentTasksManager, ConcurrentTasksManager>();

            return services;
        }
    }
}
