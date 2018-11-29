using Blog.Core.IRepository;
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Services.Base;
namespace Blog.Core.Services
{
    public partial class PasswordLibServices : BaseServices<PasswordLib>, IPasswordLibServices
    {
        IPasswordLibRepository dal;
        public PasswordLibServices(IPasswordLibRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }

    }
}
