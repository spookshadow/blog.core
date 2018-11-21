using System;
using Blog.Core.Common.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class MemoryCacheManager : ICacheManager
    {
        //引用Microsoft.Extensions.Caching.Memory;这个和.net 还是不一样，没有了Httpruntime了
        private IMemoryCache _cache;
        /// <summary>
        /// 还是通过构造函数的方法，获取
        /// </summary>
        public MemoryCacheManager(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Clear()
        {
            
        }

        public bool Exists(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public object Get(string cacheKey)
        {
            return _cache.Get(cacheKey);
        }

        public TEntity Get<TEntity>(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, object value, TimeSpan cacheTime)
        {
            _cache.Set(key, value, TimeSpan.FromSeconds(7200));
        }
    }
}
