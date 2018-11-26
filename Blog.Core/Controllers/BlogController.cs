using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Core.Common.Cache;
using Blog.Core.IRepository;
using Blog.Core.IServices;
using Blog.Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Core
{
    /// <summary>
    /// 测试路由
    /// </summary>
    [Produces("application/json")]
    [Route("api/Blog")]
    [Authorize(Policy = "Admin")]
    public class BlogController : Controller
    {
        IAdvertisementServices advertisementServices;
        IAdvertisementRepository advertisementRepository;

        ICacheManager cacheManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="advertisementServices"></param>
        /// <param name="advertisementRepository"></param>
        /// <param name="cacheManager"></param>
        public BlogController(IAdvertisementServices advertisementServices, IAdvertisementRepository advertisementRepository, ICacheManager cacheManager)
        {
            this.advertisementServices = advertisementServices;
            this.advertisementRepository = advertisementRepository;
            this.cacheManager = cacheManager;
        }

        /// <summary>
        /// 测试Get
        /// GET: api/Blog
        /// </summary>
        [HttpGet("{id}", Name = "Get")]
        public async Task<List<Advertisement>> Get(int id)
        {
            var result = await advertisementServices.Query(d => d.Id == id);
            return result;
        }
    }
}