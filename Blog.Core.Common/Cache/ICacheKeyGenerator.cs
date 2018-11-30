using System.Reflection;

namespace Blog.Core.Common.Cache
{
    public interface ICacheKeyGenerator
    {
        string GeneratorKey(MethodInfo methodInfo, object[] args, string customKey = "", string prefix = "");
    }
}