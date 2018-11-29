using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Blog.Core.AuthHelper.OverWrite
{
    /// <summary>
    /// 
    /// </summary>
    public class JwtTokenAuth
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public JwtTokenAuth(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 过滤每一个http请求
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext httpContext)
        {
            try
            {
                //检测是否包含'Authorization'请求头
                if (!httpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    return _next(httpContext);
                }
                var tokenHeader = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                //序列化token，获取授权
                TokenModelJWT tm = JwtHelper.SerializeJWT(tokenHeader);
                Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxx" + tm.Uid);
                Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxx" + tm.Role);

                //授权
                var claimList = new List<Claim>();
                var claim = new Claim(ClaimTypes.Role, tm.Role);
                claimList.Add(claim);
                var identity = new ClaimsIdentity(claimList);
                var principal = new ClaimsPrincipal(identity);
                httpContext.User = principal;

                return _next(httpContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return httpContext.Response.WriteAsync(ex.Message);
            }
        }

    }
}