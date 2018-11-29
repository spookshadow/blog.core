using System;
using System.Collections.Generic;
using Blog.Core.SwaggerHelper;
using Microsoft.AspNetCore.Mvc;
using static Blog.Core.SwaggerHelper.CustomApiVersion;

namespace Blog.Core.Controllers.v1
{
    /// <summary>
    /// ????
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ApbController : ControllerBase
    {
        /// <summary>
        /// Test Get
        /// </summary>
        [HttpGet]
        [CustomRoute(ApiVersions.v1, "apbs")]
        public IEnumerable<string> Get()
        {
            return new string[] { "V1" };
        }
    }
}