using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Model.Models
{
    /// <summary>
    /// 博客文章
    /// </summary>
    public class Topic
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public Topic()
        {
            this.TopicDetail = new List<TopicDetail>();
            this.Updatetime = DateTime.Now;
        }
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }
        
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }
        public string Author { get; set; }
        public string SectendDetail { get; set; }
        public bool IsDelete { get; set; }
        public int Read { get; set; }
        public int Commend { get; set; }
        public int Good { get; set; }
        public DateTime Createtime { get; set; }
        public DateTime Updatetime { get; set; }

        public virtual ICollection<TopicDetail> TopicDetail { get; set; }
    }
}
