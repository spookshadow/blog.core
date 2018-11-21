namespace Blog.Core
{
    /// <summary>
    /// 简单的缓存接口，只有查询和添加，以后会进行扩展
    /// </summary>
    public interface ICaching
    {
        /// <summary>
        /// 
        /// </summary>
        object Get(string cacheKey);

        /// <summary>
        /// 
        /// </summary>
        void Set(string cacheKey, object cacheValue);
    }
}