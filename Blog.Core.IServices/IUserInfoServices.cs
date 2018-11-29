using System.Threading.Tasks;
using Blog.Core.Model.Models;
using Blog.Core.IServices.Base;
namespace Blog.Core.IServices
{
    /// <summary>
    /// UserInfoServices
    /// </summary>	
    public interface IUserInfoServices :IBaseServices<UserInfo>
	{
        Task<UserInfo> SaveUserInfo(string loginName, string loginPWD);
        Task<string> GetUserRoleNameStr(string loginName, string loginPWD);
    }
}
