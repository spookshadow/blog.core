using System;

namespace Blog.Core.Model.Models
{
    /// <summary>
    /// 留言表
    /// </summary>
    public class GuestBook
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 博客ID
        /// </summary>
        public int? BlogId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// qq
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 留言内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 是否显示在前台,0否1是
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// 文章
        /// </summary>
        public BlogArticle BlogArticle { get; set; }
    }
}
