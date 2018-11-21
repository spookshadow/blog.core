using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="advertisementServices"></param>
        public BlogController(IAdvertisementServices advertisementServices)
        {
            this.advertisementServices = advertisementServices;
        }

        /// <summary>
        /// 测试Get
        /// GET: api/Blog
        /// </summary>
        [HttpGet("{id}", Name = "Get")]
        public async Task<List<Advertisement>> Get(int id)
        {
            return await advertisementServices.Query(d => d.Id == id);
        }
    }
}