﻿namespace Blog.Core.Model.VeiwModels
{
    /// <summary>
    /// 留言排名展示类
    /// </summary>
    public class TopBlogVM
    {
        /// <summary>
        /// 博客ID
        /// </summary>
        public int? BlogId { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int Counts { get; set; }

        /// <summary>
        /// 博客标题
        /// </summary>
        public string Title { get; set; }
    }
}
