using Blog.Core.Common;
using Blog.Core.Common.Cache;
using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blog.Core.AOP
{
    /// <summary>
    /// 面向切面的缓存使用
    /// </summary>
    public class BlogCacheAOP : IInterceptor
    {
        #region 构造器
        //通过注入的方式，把缓存操作接口通过构造函数注入
        private ICacheManager _cache;
        /// <summary>
        /// 构造器
        /// </summary>
        public BlogCacheAOP(ICacheManager cache)
        {
            _cache = cache;
        }
        #endregion

        #region 缓存处理方法定义
        /*
        * 假如Method属于Class (Type=A) 的实例obj，方法签名为
        * public void Method<T>(int value) 
        * 那么：想调用obj.Method<Person>(5);
        * A obj = new A();
        * Type t = obj.GetType();
        * MethodInfo mi = t.GetMethod("Method").MakeGenericMethod(typeof(Person));
        * mi.Invoke(obj, new object[] { 5 });
        */
        private static readonly MethodInfo setTaskTCacheMethodInfo = typeof(BlogCacheAOP).GetMethod("SetTaskTCache", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo getTaskTCacheMethodInfo = typeof(BlogCacheAOP).GetMethod("GetTaskTCache", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo getCacheMethodInfo = typeof(BlogCacheAOP).GetMethod("GetCache", BindingFlags.Instance | BindingFlags.NonPublic);


        #endregion

        /// <summary>
        /// 拦截处理方法
        /// </summary>
        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            // 通过方法属性判断是否执行缓存操作
            var cacheAttribute = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingAttribute)) as CachingAttribute;
            if (cacheAttribute == null)
            {
                invocation.Proceed();//直接执行被拦截方法
                return;
            }

            // 获取执行方法类型
            var delegateType = GetDelegateType(invocation);
            // Task ，无返回值，直接执行
            if (delegateType == MethodType.AsyncAction)
            {
                invocation.Proceed();
                return;
            }

            // 获取绝对过期时间
            var timeout = cacheAttribute.AbsoluteExpiration.ObjToInt();

            #region 缓存存在返回缓存
            var cacheKey = CustomCacheKey(invocation);
            if (_cache.Exists(cacheKey))
            {
                if (delegateType == MethodType.Synchronous)
                {
                    var type = invocation.Method.ReturnType;
                    var cacheResult = _cache.Get(cacheKey, type);
                    var mi = getCacheMethodInfo.MakeGenericMethod(type);
                    invocation.ReturnValue = mi.Invoke(this, new[] { cacheResult });
                }

                if (delegateType == MethodType.AsyncFunction)
                {
                    var type = invocation.Method.ReturnType.GetGenericArguments()[0];
                    var cacheResult = _cache.Get(cacheKey, type);
                    var mi = getTaskTCacheMethodInfo.MakeGenericMethod(type);
                    invocation.ReturnValue = mi.Invoke(this, new[] { cacheResult });
                }
                return;
            }
            #endregion

            #region 将执行结果存缓存
            // Synchronous
            if (delegateType == MethodType.Synchronous)
            {
                invocation.Proceed();
                _cache.Set(cacheKey, invocation.ReturnValue, TimeSpan.FromMinutes(timeout));
            }
            // Task<T>
            if (delegateType == MethodType.AsyncFunction)
            {
                invocation.Proceed();
                var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
                var mi = setTaskTCacheMethodInfo.MakeGenericMethod(resultType);
                invocation.ReturnValue = mi.Invoke(this, new[] { invocation.ReturnValue, cacheKey, timeout });
            }
            #endregion
        }

        #region 缓存操作
        private T GetCache<T>(Object result)
        {
            return (T)result;
        }
        #endregion

        #region Task<T>缓存处理
        /// <summary>
        /// 将异步执行结果存缓存
        /// </summary>
        private async Task<T> SetTaskTCache<T>(Task<T> task, string cacheKey, int timeout)
        {
            T result = await task.ConfigureAwait(false);
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(timeout));
            return result;
        }

        /// <summary>
        /// 重新包装缓存结果为Task
        /// </summary>
        private async Task<T> GetTaskTCache<T>(Object result)
        {
            return await Task<T>.Run(() => { return (T)result; });
        }
        #endregion

        #region 获取方法类型
        private MethodType GetDelegateType(IInvocation invocation)
        {
            var returnType = invocation.Method.ReturnType;
            if (returnType == typeof(Task))
                return MethodType.AsyncAction;
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                return MethodType.AsyncFunction;
            return MethodType.Synchronous;
        }

        private enum MethodType
        {
            Synchronous,
            AsyncAction,
            AsyncFunction
        }
        #endregion

        #region 自定义缓存key
        private string CustomCacheKey(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;
            var methodArguments = invocation.Arguments.Select(GetArgumentValue).ToList();//获取参数列表，最多三个

            string key = $"{typeName}:{methodName}:";
            foreach (var param in methodArguments)
            {
                key += $"{param}:";
            }

            return key.TrimEnd(':');
        }

        private string GetArgumentValue(object arg)
        {
            if (arg is int || arg is long || arg is string)
                return arg.ToString();

            if (arg is DateTime)
                return ((DateTime)arg).ToString("yyyyMMddHHmmss");

            return "";
        }
        #endregion
    }

}
