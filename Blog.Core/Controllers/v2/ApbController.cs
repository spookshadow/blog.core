using System;
using System.Collections.Generic;
using Blog.Core.SwaggerHelper;
using Microsoft.AspNetCore.Mvc;
using static Blog.Core.SwaggerHelper.CustomApiVersion;

namespace Blog.Core.Controllers.v2
{
    /// <summary>
    /// V2
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ApbController : ControllerBase
    {
        /// <summary>
        /// Test Get
        /// </summary>
        [HttpGet]
        [CustomRoute(ApiVersions.v2, "apbs")]
        public IEnumerable<string> Get()
        {
            return new string[] { "V2" };
        }
    }
}