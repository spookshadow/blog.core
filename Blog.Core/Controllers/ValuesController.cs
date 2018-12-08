using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blog.Core.IServices;
using Blog.Core.Model;
using Blog.Core.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Mono.Linq.Expressions;

namespace Blog.Core.Controllers
{
    /// <summary>
    /// Values控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    // [Authorize(Roles = "Admin,Client")]
    //[Authorize(Policy = "SystemOrAdmin")]
    public class ValuesController : ControllerBase
    {
        private IAdvertisementServices services;

        public ValuesController(IAdvertisementServices services)
        {
            this.services = services;
        }

        /// <summary>
        /// Get方法
        /// </summary>
        /// <returns></returns>
        // GET api/values
        [HttpGet]
        public async Task<List<Advertisement>> Get(int id)
        {
            Expression<Func<Advertisement, bool>> exp = a => a.Id > 18 && a.ImgUrl.Contains("https");
            Expression<Func<Advertisement, bool>> exp2 = a => a.Id > id && a.ImgUrl.Contains("https");

            var s1 = exp.ToString();
            var s2 = exp2.ToString();


            return await services.Query(a => a.Id > id && a.ImgUrl.Contains("https"));
        }
        /// <summary>
        /// post
        /// </summary>
        /// <param name="blogArticle">model实体类参数</param>
        [HttpPost]
        [AllowAnonymous]
        public object Post([FromBody]  BlogArticle blogArticle)
        {
            return Ok(new { success = true, data = blogArticle });
        }
        /// <summary>
        /// Put方法
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        /// <summary>
        /// Delete方法
        /// </summary>
        /// <param name="id"></param>
        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        /// <summary>
        /// Get方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTest")]
        public List<Advertisement> GetTest(int id)
        {
            return services.QueryTest(id);
        }

        /// <summary>
        /// Get方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTestAction")]
        public async Task GetTestAction(int id)
        {
            await services.ActionTest(id);
        }
    }
}
