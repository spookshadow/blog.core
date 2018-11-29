using Blog.Core.Model.Models;
using System.Threading.Tasks;
using Blog.Core.IServices.Base;
namespace Blog.Core.IServices
{
    /// <summary>
    /// RoleServices
    /// </summary>	
    public interface IRoleServices :IBaseServices<Role>
	{
        Task<Role> SaveRole(string roleName);
    }
}
