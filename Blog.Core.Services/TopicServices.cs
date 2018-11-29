using Blog.Core.IRepository;
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Services.Base;
namespace Blog.Core.Services
{
    public class TopicServices: BaseServices<Topic>, ITopicServices
    {

        ITopicRepository dal;
        public TopicServices(ITopicRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }


    }
}
