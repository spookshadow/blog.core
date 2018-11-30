using System;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Blog.Core.Common.Cache;

namespace Blog.Core.Common.Interceptor
{
    public class ServicesCacheInterceptor : AbstractInterceptorAttribute
    {
        #region 构造器
        //通过注入的方式，把缓存操作接口通过构造函数注入
        private ICacheManager _cache;
        private ICacheKeyGenerator _cacheKeyGen;
        private CacheOptions _cacheOptions;
        /// <summary>
        /// 构造器
        /// </summary>
        public ServicesCacheInterceptor(ICacheManager cache, ICacheKeyGenerator cacheKeyGen, CacheOptions options)
        {
            _cache = cache;
            _cacheKeyGen = cacheKeyGen;
            _cacheOptions = options;
        }
        #endregion

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var method = context.ImplementationMethod;
            #region 不执行缓存操作
            if (method.ReturnType == typeof(void) || context.IsAsync() && method.ReturnType == typeof(Task))
            {
                await next(context);
                return;
            }

            // 通过方法属性判断是否执行缓存操作
            var attribute = method.GetCustomAttributes(true)
                            .FirstOrDefault(p => p.GetType() == typeof(CachingAttribute)) as CachingAttribute;
            if (attribute == null)
            {
                await next(context);
                return;
            }
            #endregion

            //获取自定义缓存键
            var cacheKey = _cacheKeyGen.GeneratorKey(method, context.Parameters, attribute?.CacheKey, _cacheOptions?.CacheKeyPrefix);

            var returnType = context.IsAsync()
                ? method.ReturnType.GetGenericArguments().First()
                : method.ReturnType;

            //根据key获取相应的缓存值
            if (_cache.Exists(cacheKey))
            {
                var cacheValue = _cache.Get(cacheKey, returnType);
                if (context.IsAsync())
                {

                    var cacheResult = Task.Run(() => { return cacheValue; });
                    //将当前获取到的缓存值，赋值给当前执行方法
                    context.ReturnValue = cacheResult;
                    await context.Complete();
                    return;
                }
                else
                {
                    context.ReturnValue = cacheValue;
                    return;
                }
            }
            //去执行当前的方法
            await next(context);

            var timeout = attribute.AbsoluteExpiration.ObjToInt();
            if (context.IsAsync())
            {
                Task<object> task = context.UnwrapAsyncReturnValue();
                _cache.Set(cacheKey, task.Result, TimeSpan.FromMinutes(timeout));
            }
            else
            {
                _cache.Set(cacheKey, context.ReturnValue, TimeSpan.FromMinutes(timeout));
            }
        }
    }
}