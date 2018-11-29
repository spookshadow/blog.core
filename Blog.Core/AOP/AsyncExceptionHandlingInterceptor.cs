using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Blog.Core.AOP
{
    public class AsyncExceptionHandlingInterceptor : IInterceptor
    {
        private static readonly MethodInfo handleAsyncMethodInfo = typeof(AsyncExceptionHandlingInterceptor).GetMethod("HandleAsyncWithResult", BindingFlags.Instance | BindingFlags.NonPublic);
        /*private readonly IExceptionHandler _handler;

        public AsyncExceptionHandlingInterceptor(IExceptionHandler handler)
        {
            _handler = handler;
        } */

        public void Intercept(IInvocation invocation)
        {
            var delegateType = GetDelegateType(invocation);
            // 同步
            if (delegateType == MethodType.Synchronous)
            {
                invocation.Proceed();
            }
            // Task
            if (delegateType == MethodType.AsyncAction)
            {
                invocation.Proceed();
                Func<Task> continuation = async () =>
                {
                    await (Task)invocation.ReturnValue;
                };
                invocation.ReturnValue = continuation();
            }
            // Task<T>
            if (delegateType == MethodType.AsyncFunction)
            {
                invocation.Proceed();
                var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
                var mi = handleAsyncMethodInfo.MakeGenericMethod(resultType);
                invocation.ReturnValue = mi.Invoke(this, new[] { invocation.ReturnValue });
            }

            var result = invocation.ReturnValue;

        }

        private async Task<T> HandleAsyncWithResult<T>(Task<T> task)
        {
            var result = await task;
            return result;
            //return await _handler.HandleExceptions(async () => await task);
        }

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
    }
}
