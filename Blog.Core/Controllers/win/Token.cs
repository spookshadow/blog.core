using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Core.Controllers
{
    public class ApiToken
    { 
        /// <summary>
        /// 授权码
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// 用户名对应签名Token
        /// </summary>
        public Guid SignToken { get; set; }


        /// <summary>
        /// Token过期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }
    }
}