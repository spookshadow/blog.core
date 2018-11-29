using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Model.Models
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public UserInfo() { }

        /// <summary>
        /// 构造器
        /// </summary>
        public UserInfo(string loginName, string loginPWD)
        {
            LoginName = loginName;
            LoginPWD = loginPWD;
            RealName = LoginName;
            Status = 0;
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
            LastErrTime = DateTime.Now;
            ErrorCount = 0;

        }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 登录账号
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPWD { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        ///最后登录时间 
        /// </summary>
        public DateTime LastErrTime { get; set; }

        /// <summary>
        ///错误次数 
        /// </summary>
        public int ErrorCount { get; set; }

    }
}
