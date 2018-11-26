using System;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Blog.Core.Common.Cache;

namespace Blog.Core.Common.Interceptor
{
    public class ServicesCacheInterceptor : AbstractInterceptorAttribute
    {
        //通过注入的方式，把缓存操作接口通过构造函数注入
        private ICacheManager _cache;
        /// <summary>
        /// 构造器注入 cache实现
        /// </summary>
        public ServicesCacheInterceptor(ICacheManager cache)
        {
            _cache = cache;
        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {

                //获取自定义缓存键
                var cacheKey = CustomCacheKey(context);
                //根据key获取相应的缓存值
                if (_cache.Exists(cacheKey))
                {
                    var type = _cache.Get<Type>(cacheKey + "_type");
                    var cacheValue = _cache.Get(cacheKey, type);
                    var cacheResult = Task.Run(() => { return cacheValue; });
                    //将当前获取到的缓存值，赋值给当前执行方法
                    context.ReturnValue = cacheResult;
                    await context.Complete();
                    return;
                }
                //去执行当前的方法
                await next(context);

                //存入缓存
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    Task<object> task = context.UnwrapAsyncReturnValue();
                    _cache.Set(cacheKey, task.Result, TimeSpan.FromHours(2));
                    _cache.Set(cacheKey + "_type", task.Result.GetType(), TimeSpan.FromHours(2));
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Service threw an exception!");
                throw;
            }
            finally
            {

                Console.WriteLine("After service call");
            }
        }

        //自定义缓存键
        private string CustomCacheKey(AspectContext context)
        {
            var typeName = context.GetType();
            var methodName = context.ServiceMethod.Name;
            var methodArguments = context.Parameters.Select(GetArgumentValue).Take(3).ToList();//获取参数列表，最多三个

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