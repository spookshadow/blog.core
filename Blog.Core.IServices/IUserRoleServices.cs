using Blog.Core.Model.Models;
using System.Threading.Tasks;
using Blog.Core.IServices.Base;
namespace Blog.Core.IServices
{
    /// <summary>
    /// UserRoleServices
    /// </summary>	
    public interface IUserRoleServices :IBaseServices<UserRole>
	{

        Task<UserRole> SaveUserRole(int uid, int rid);
    }
}

