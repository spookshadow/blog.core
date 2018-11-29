using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.IServices.Base;
using Blog.Core.Model.Models;

namespace Blog.Core.IServices
{
    public interface IAdvertisementServices : IBaseServices<Advertisement>
    {
        List<Advertisement> QueryTest(int id);
        Task ActionTest(int id);
    }
}
