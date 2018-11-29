using System;

namespace Blog.Core.Model.Models
{
    /// <summary>
    /// 密码库表
    /// </summary>
    public class PasswordLib
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        public bool? IsDeleted { get; set; }
        public string URL { get; set; }
        public string PWD { get; set; }
        public string AccountName { get; set; }
        public int? Status { get; set; }
        public int? ErrorCount { get; set; }
        public string HintPwd { get; set; }
        public string Hintquestion { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? LastErrTime { get; set; }
    }
}
