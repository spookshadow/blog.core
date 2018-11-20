using System.Collections.Generic;
using Blog.Core.IServices;
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
        [HttpGet]
        public int Get(int i, int j)
        {
            IAdvertisementServices advertisementServices = new AdvertisementServices();
            return advertisementServices.Sum(i, j);
        }
    }
}

