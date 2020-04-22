using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ToolsToLive.ConcurrentCache
{
    public static class Extensions
    {
        /// <summary>
        ///  Can be used to deep copy of an object. But it takes time, so probably not so good idea to use it along with fast cache.
        /// </summary>
        public static T DeepCopy<T>(this T source)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, source);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
