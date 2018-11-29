using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.Common.Cache;
using Blog.Core.Common.Helper;
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.SwaggerHelper;
using Microsoft.AspNetCore.Mvc;
using static Blog.Core.SwaggerHelper.CustomApiVersion;

namespace Blog.Core.Controllers
{
    /// <summary>
    /// Blog控制器所有接口
    /// </summary>
    [Produces("application/json")]
    [Route("api/Blog")]
    public class BlogController : Controller
    {
        IAdvertisementServices advertisementServices;
        IBlogArticleServices blogArticleServices;
        ICacheManager cacheManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="advertisementServices"></param>
        /// <param name="blogArticleServices"></param>
        /// <param name="cacheManager"></param>
        public BlogController(IAdvertisementServices advertisementServices, IBlogArticleServices blogArticleServices, ICacheManager cacheManager)
        {
            this.advertisementServices = advertisementServices;
            this.blogArticleServices = blogArticleServices;
            this.cacheManager = cacheManager;
        }


        /// <summary>
        /// 获取博客列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> Get(int id, int page = 1, string category = "技术博文")
        {
            int intTotalCount = 6;
            int TotalCount = 1;
            List<BlogArticle> blogArticleList = new List<BlogArticle>();

            if (cacheManager.GetT<object>("Redis.Blog") != null)
            {
                blogArticleList = cacheManager.GetT<List<BlogArticle>>("Redis.Blog");
            }
            else
            {
                blogArticleList = await blogArticleServices.Query(a => a.Category == category);
                cacheManager.Set("Redis.Blog", blogArticleList, TimeSpan.FromHours(2));
            }


            TotalCount = blogArticleList.Count() / intTotalCount;

            blogArticleList = blogArticleList.OrderByDescending(d => d.Id).Skip((page - 1) * intTotalCount).Take(intTotalCount).ToList();

            foreach (var item in blogArticleList)
            {
                if (!string.IsNullOrEmpty(item.Content))
                {
                    item.Remark = (HtmlHelper.ReplaceHtmlTag(item.Content)).Length >= 200 ? (HtmlHelper.ReplaceHtmlTag(item.Content)).Substring(0, 200) : (HtmlHelper.ReplaceHtmlTag(item.Content));
                    int totalLength = 500;
                    if (item.Content.Length > totalLength)
                    {
                        item.Content = item.Content.Substring(0, totalLength);
                    }
                }
            }

            return Ok(new
            {
                success = true,
                page = page,
                pageCount = TotalCount,
                data = blogArticleList
            });
        }


        // GET: api/Blog/5
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "Get")]
        //[Authorize(Policy = "Admin")]
        public async Task<object> Get(int id)
        {
            var model = await blogArticleServices.getBlogDetails(id);
            return Ok(new
            {
                success = true,
                data = model
            });
        }


        /// <summary>
        /// 获取博客测试信息 v2版本
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        ////MVC自带特性 对 api 进行组管理
        //[ApiExplorerSettings(GroupName = "v2")]
        ////路径 如果以 / 开头，表示绝对路径，反之相对 controller 的想u地路径
        //[Route("/api/v2/blog/Blogtest")]

        //和上边的版本控制以及路由地址都是一样的
        [CustomRoute(ApiVersions.v2, "Blogtest")]
        public ActionResult V2_Blogtest()
        {
            return Ok(new { status = 220, data = "我是第二版的博客信息" });
        }



    }
}
