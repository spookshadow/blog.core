using System;
using Blog.Core.AuthHelper.OverWrite;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;

namespace Blog.Core.Controllers
{
    /// <summary>
    /// 模拟登陆
    /// </summary>
    [ApiController]
    [Route("api/Login")]
    [EnableCors("LimitRequests")] // 跨域
    public class LoginController : ControllerBase
    {
        /// <summary>
        /// 获取JWT的重写方法，推荐这种，注意在文件夹OverWrite下
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="sub">角色</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Token")]
        public JsonResult GetJWTStr(long id = 1, string sub = "Admin")
        {
            //这里就是用户登陆以后，通过数据库去调取数据，分配权限的操作
            TokenModelJWT tokenModel = new TokenModelJWT();
            tokenModel.Uid = id;
            tokenModel.Role = sub;

            string jwtStr = JwtHelper.IssueJWT(tokenModel);
            return new JsonResult(jwtStr);
        }

        /// <summary>
        /// JSONP跨域实现
        /// www testCORS.html 发送跨域请求
        /// </summary>
        [HttpGet]
        [Route("jsonp")]
        public void Getjsonp(string callBack, long id = 1, string sub = "Admin", int expiresSliding = 30, int expiresAbsoulute = 30)
        {
            TokenModelJWT tokenModel = new TokenModelJWT();
            tokenModel.Uid = id;
            tokenModel.Role = sub;

            DateTime d1 = DateTime.Now;
            DateTime d2 = d1.AddMinutes(expiresSliding);
            DateTime d3 = d1.AddDays(expiresAbsoulute);
            TimeSpan sliding = d2 - d1;
            TimeSpan absoulute = d3 - d1;

            string jwtStr = JwtHelper.IssueJWT(tokenModel);

　　　　　　  //重要，一定要这么写
            string response = string.Format("\"value\":\"{0}\"", jwtStr);
            string call = callBack + "({"+response+"})";
            Response.WriteAsync(call);
        }
    }
}
