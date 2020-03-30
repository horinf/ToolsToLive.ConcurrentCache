namespace ToolsToLive.ConcurrentCache.Model
{
    /// <summary>
    /// A wrapper for data stored in a cache containing the data itself.
    /// </summary>
    /// <typeparam name="T">Type of cached value.</typeparam>
    public class CacheData<T>
    {
        /// <summary>
        /// Value.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">Value for cache.</param>
        public CacheData(T value)
        {
            Value = value;
        }
    }
}
