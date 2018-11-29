using Blog.Core.IRepository;
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Services.Base;
namespace Blog.Core.Services
{
    public class TopicDetailServices: BaseServices<TopicDetail>, ITopicDetailServices
    {
        ITopicDetailRepository dal;
        public TopicDetailServices(ITopicDetailRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }

    }
}
