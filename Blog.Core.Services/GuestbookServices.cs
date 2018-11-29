using Blog.Core.IRepository;
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Services.Base;
namespace Blog.Core.Services
{
    public class GuestBookServices : BaseServices<GuestBook>, IGuestBookServices
    {
        IGuestBookRepository dal;
        public GuestBookServices(IGuestBookRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }
    }
}