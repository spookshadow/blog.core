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
    public class TopicDetail
    {
        public TopicDetail()
        {
            this.Updatetime = DateTime.Now;
        }
        public int Id { get; set; }
        public int TopicId { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Detail { get; set; }
        public string SectendDetail { get; set; }
        public bool IsDelete { get; set; }
        public int Read { get; set; }
        public int Commend { get; set; }
        public int Good { get; set; }
        public DateTime Createtime { get; set; }
        public DateTime Updatetime { get; set; }
        public int Top { get; set; }

        public virtual Topic Topic { get; set; }

    }
}
