using System;

namespace Blog.Core.Common.Cache
{
    public interface ICacheManager
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        TEntity GetT<TEntity>(string key);

        Object Get(string key, Type type);
        //设置
        void Set(string key, object value, TimeSpan cacheTime);
        //判断是否存在
        bool Exists(string key);
        //移除
        void Remove(string key);
        //清除
        void Clear();
    }
}
