using Blog.Core.Model.Models;
using Blog.Core.Model.VeiwModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Core.IServices.Base;
namespace Blog.Core.IServices
{
    public interface IBlogArticleServices : IBaseServices<BlogArticle>
    {
        Task<List<BlogArticle>> getBlogs();
        Task<BlogVM> getBlogDetails(int id);
    }
}