using System.Linq;
using System.Reflection;
using Blog.Core.Common.Helper;

namespace Blog.Core.Common.Cache
{
    public class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        private const char LinkChar = ':';

        public string GeneratorKey(MethodInfo methodInfo, object[] args, string customKey = "", string prefix = "")
        {
            var attribute =
                methodInfo.GetCustomAttributes(true).FirstOrDefault(p => p.GetType() == typeof(CachingAttribute))
                    as CachingAttribute;
            if (attribute == null || string.IsNullOrWhiteSpace(attribute.CacheKey))
            {
                var typeName = methodInfo.DeclaringType?.FullName;
                var methodName = methodInfo.Name;
                return
                    $"{(string.IsNullOrWhiteSpace(prefix) ? "" : $"{prefix}{LinkChar}")}{typeName}{LinkChar}{methodName}{LinkChar}{ExtensionMethod.MD5(ExtensionMethod.AsBytes(args))}";
            }
            return string.Format(attribute.CacheKey, args);
        }
    }
}