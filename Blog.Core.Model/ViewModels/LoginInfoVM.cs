namespace Blog.Core.Model.VeiwModels
{
    /// <summary>
    /// 登录信息
    /// </summary>
    public class LoginInfoVM
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string LoginPwd { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string VCode { get; set; }
        /// <summary>
        /// 是否会员
        /// </summary>
        public bool IsMember { get; set; }
    }
}
