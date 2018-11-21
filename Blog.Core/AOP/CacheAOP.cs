using System;
using System.Collections.Generic;
using System.Linq;
using Blog.Core.Common;
using Blog.Core.Common.Cache;
using Blog.Core.Model;
using Castle.DynamicProxy;

namespace Blog.Core
{
    /// <summary>
    /// 缓存切面
    /// </summary>
    public class CacheAOP : IInterceptor
    {
        //通过注入的方式，把缓存操作接口通过构造函数注入
        private ICacheManager _cache;
        /// <summary>
        /// 构造器注入 cache实现
        /// </summary>
        public CacheAOP(ICacheManager cache)
        {
            _cache = cache;
        }
        /// <summary>
        /// Intercept方法是拦截的关键所在，也是IInterceptor接口中的唯一定义
        /// </summary>
        public void Intercept(IInvocation invocation)
        {
            #region 验证当前方法Attribute,存在CachingAttribute 执行缓存
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            var qCachingAttribute = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingAttribute)) as CachingAttribute;
            if (qCachingAttribute == null)
            {
                invocation.Proceed();
                return;
            }
            #endregion

            //获取自定义缓存键
            var cacheKey = CustomCacheKey(invocation);
            //根据key获取相应的缓存值
            if (_cache.Exists(cacheKey))
            {
                var cacheValue = _cache.Get<List<Advertisement>>(cacheKey);
                //将当前获取到的缓存值，赋值给当前执行方法
                invocation.ReturnValue = cacheValue;
                return;
            }
            //去执行当前的方法
            invocation.Proceed();
            //存入缓存
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                _cache.Set(cacheKey, invocation.ReturnValue, TimeSpan.FromHours(2));
            }
        }

        //自定义缓存键
        private string CustomCacheKey(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;
            var methodArguments = invocation.Arguments.Select(GetArgumentValue).Take(3).ToList();//获取参数列表，最多三个

            string key = $"{typeName}:{methodName}:";
            foreach (var param in methodArguments)
            {
                key += $"{param}:";
            }

            return key.TrimEnd(':');
        }
        //object 转 string
        private string GetArgumentValue(object arg)
        {
            if (arg is int || arg is long || arg is string)
                return arg.ToString();

            if (arg is DateTime)
                return ((DateTime)arg).ToString("yyyyMMddHHmmss");

            return "";
        }

    }
}
