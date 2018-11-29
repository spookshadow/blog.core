using Blog.Core.IRepository;
using Blog.Core.Model.Models;
using Blog.Core.Repository.Base;
namespace Blog.Core.Repository
{
    /// <summary>
    /// UserInfoRepository
    /// </summary>	
    public class UserInfoRepository : BaseRepository<UserInfo>, IUserInfoRepository
    {
    }
}
