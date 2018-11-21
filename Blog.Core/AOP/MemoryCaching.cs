using System;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class MemoryCaching : ICaching
    {
        //引用Microsoft.Extensions.Caching.Memory;这个和.net 还是不一样，没有了Httpruntime了
        private IMemoryCache _cache;
        /// <summary>
        /// 还是通过构造函数的方法，获取
        /// </summary>
        public MemoryCaching(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Get(string cacheKey)
        {
            return _cache.Get(cacheKey);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Set(string cacheKey, object cacheValue)
        {
            _cache.Set(cacheKey, cacheValue, TimeSpan.FromSeconds(7200));
        }
    }
}
