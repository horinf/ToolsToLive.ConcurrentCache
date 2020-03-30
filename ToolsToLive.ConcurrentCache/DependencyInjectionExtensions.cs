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
            services.AddScoped<IConcurrentCache, ConcurrentCache>();
            services.AddScoped<IConcurrentCacheTasksManager, ConcurrentCacheTasksManager>();
            services.AddScoped<IConcurrentTasksManager, ConcurrentTasksManager>();

            return services;
        }
    }
}
