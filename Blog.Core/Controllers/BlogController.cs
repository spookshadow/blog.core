using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Core.IServices;
using Blog.Core.Model;
using Blog.Core.Services;
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
        /// <summary>
        /// 测试Get
        /// GET: api/Blog
        /// </summary>
        [HttpGet("{id}", Name = "Get")]
        public async Task<List<Advertisement>> Get(int id)
        {
            IAdvertisementServices advertisementServices = new AdvertisementServices();

            return await advertisementServices.Query(d => d.Id == id);
        }
    }
}

