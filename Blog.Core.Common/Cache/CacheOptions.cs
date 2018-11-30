using System;

namespace Blog.Core.Common.Cache
{
    public class CacheOptions
    {
        ///<summary>
        /// 缓存Key值前缀
        ///</summary>
        public string CacheKeyPrefix { get; set; } = "Blog.Core";

        
    }
}
