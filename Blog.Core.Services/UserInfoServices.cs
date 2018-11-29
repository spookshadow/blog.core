using Blog.Core.IServices;
using Blog.Core.IRepository;
using System.Threading.Tasks;
using System.Linq;
using Blog.Core.Model.Models;
using Blog.Core.Services;
using Blog.Core.Services.Base;
namespace Blog.Core.FrameWork.Services
{
    /// <summary>
    /// UserInfoServices
    /// </summary>	
    public class UserInfoServices : BaseServices<UserInfo>, IUserInfoServices
    {

        IUserInfoRepository dal;
        IUserRoleServices userRoleServices;
        IRoleRepository roleRepository;
        public UserInfoServices(IUserInfoRepository dal, IUserRoleServices userRoleServices, IRoleRepository roleRepository)
        {
            this.dal = dal;
            this.userRoleServices = userRoleServices;
            this.roleRepository = roleRepository;
            base.baseDal = dal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPWD"></param>
        /// <returns></returns>
        public async Task<UserInfo> SaveUserInfo(string loginName, string loginPWD)
        {
            UserInfo UserInfo = new UserInfo(loginName, loginPWD);
            UserInfo model = new UserInfo();
            var userList = await dal.Query(a => a.LoginName == UserInfo.LoginName && a.LoginPWD == UserInfo.LoginPWD);
            if (userList.Count > 0)
            {
                model = userList.FirstOrDefault();
            }
            else
            {
                var id = await dal.Add(UserInfo);
                model = await dal.QueryByID(id);
            }

            return model;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPWD"></param>
        /// <returns></returns>
        public async Task<string> GetUserRoleNameStr(string loginName, string loginPWD)
        {
            string roleName = "";
            var user = (await dal.Query(a => a.LoginName == loginName && a.LoginPWD == loginPWD)).FirstOrDefault();
            if (user != null)
            {
                var userRoles = await userRoleServices.Query(ur => ur.UserId == user.Id);
                if (userRoles.Count > 0)
                {
                    var roles = await roleRepository.QueryByIDs(userRoles.Select(ur => ur.RoleId.ObjToString()).ToArray());

                    roleName = string.Join(",", roles.Select(r => r.Name).ToArray());
                }
            }
            return roleName;
        }
    }
}