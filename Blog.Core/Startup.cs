using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.Caching.Memory;
using Blog.Core.AuthHelper.OverWrite;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using System.Reflection;
using Blog.Core.Common.Cache;
using AspectCore.Extensions.DependencyInjection;
using AspectCore.Configuration;
using Blog.Core.Repository;
using Blog.Core.IRepository;
using Blog.Core.IServices;
using Blog.Core.Services;
using Blog.Core.Common.Interceptor;
using log4net.Repository;
using log4net;
using log4net.Config;
using Blog.Core.Filter.Blog.Core.Filter;
using Blog.Core.Log;
using AutoMapper;
using Blog.Core.Common.Helper;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static Blog.Core.SwaggerHelper.CustomApiVersion;

namespace Blog.Core
{
    /// <summary>
    /// 项目启动文件
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// log4net 仓储库
        /// </summary>
        public static ILoggerRepository repository { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //log4net
            repository = LogManager.CreateRepository("Blog.Core");
            //指定配置文件
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));

        }


        /// <summary>
        /// 项目配置
        /// </summary>
        public IConfiguration Configuration { get; }
        private const string ApiName = "Blog.Core";


        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            #region 注入全局异常捕获
            services.AddMvc(
                o =>
                {
                    o.Filters.Add(typeof(GlobalExceptionsFilter));
                }
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            #endregion

            #region 依赖解析
            services.AddSingleton<CacheOptions>();
            services.AddScoped<ICacheManager, RedisCacheManager>();
            services.AddSingleton<ILoggerHelper, LogHelper>();
            services.AddScoped<ICacheKeyGenerator, DefaultCacheKeyGenerator>();
            #endregion

            #region Automapper
            services.AddAutoMapper(typeof(Startup));
            #endregion

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                /*c.SwaggerDoc("v1", new Info
                {
                    Version = "v0.1.0",
                    Title = "Blog.Core API",
                    Description = "项目说明文档",
                    TermsOfService = "None",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact { Name = "Blog.Core", Email = "", Url = "" }
                });*/

                //遍历出全部的版本，做文档信息展示
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    c.SwaggerDoc(version, new Info
                    {
                        // {ApiName} 定义成全局变量，方便修改
                        Version = version,
                        Title = $"{ApiName} 接口文档",
                        Description = $"{ApiName} HTTP API " + version,
                        TermsOfService = "None",
                        Contact = new Contact { Name = "Blog.Core", Email = "Blog.Core@xxx.com", Url = "https://www.jianshu.com/u/94102b59cc2a" }
                    });
                });

                #region Swagger添加文档注释
                var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Blog.Core.xml");
                c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改

                var xmlModelPath = Path.Combine(basePath, "Blog.Core.Model.xml");//这个就是Model层的xml文件名
                c.IncludeXmlComments(xmlModelPath);
                #endregion

                #region Token绑定到ConfigureServices
                //添加header验证信息
                var security = new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } }, };
                c.AddSecurityRequirement(security);
                //方案名称“Blog.Core”可自定义，上下一致即可
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });
                #endregion
            });
            #endregion

            #region 认证
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
            ).AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = true,//是否验证Audience 
                        ValidateIssuerSigningKey = true,//是否验证IssuerSigningKey 
                        ValidIssuer = "Blog.Core",
                        ValidAudience = "wr",
                        ValidateLifetime = true,//是否验证超时  当设置exp和nbf时有效 同时启用ClockSkew 
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtHelper.secretKey)),
                        //注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                }
            );
            #endregion


            #region Token服务注册
            services.AddSingleton<IMemoryCache>(factory =>
             {
                 var cache = new MemoryCache(new MemoryCacheOptions());
                 return cache;
             });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                options.AddPolicy("AdminOrClient", policy => policy.RequireRole("Admin,Client").Build());
            });
            #endregion

            #region AspectCore

            #region 依赖解析
            services.AddTransient<IAdvertisementRepository, AdvertisementRepository>();
            services.AddTransient<IAdvertisementServices, AdvertisementServices>();
            #endregion

            #region 全局拦截器
            /*services.AddTransient<ServicesCacheInterceptor>();
            services.ConfigureDynamicProxy(config =>
                {
                    config.Interceptors.AddServiced<ServicesCacheInterceptor>(Predicates.ForService("*Services*"));
                    config.NonAspectPredicates.AddMethod("Add*");
                    config.NonAspectPredicates.AddMethod("Update*");
                    config.NonAspectPredicates.AddMethod("Delete*");
                }
            ); */
            #endregion



            #endregion

            #region CORS
            services.AddCors(c =>
            {
                //↓↓↓↓↓↓↓注意正式环境不要使用这种全开放的处理↓↓↓↓↓↓↓↓↓↓
                /*c.AddPolicy("AllRequests", policy =>
                {
                    policy
                    .AllowAnyOrigin()//允许任何源
                    .AllowAnyMethod()//允许任何方式
                    .AllowAnyHeader()//允许任何头
                    .AllowCredentials();//允许cookie
                }); */
                //↑↑↑↑↑↑↑注意正式环境不要使用这种全开放的处理↑↑↑↑↑↑↑↑↑↑


                //一般采用这种方法
                c.AddPolicy("LimitRequests", policy =>
                {
                    policy
                    .WithOrigins("http://localhost:8090", "http://blog.core.xxx.com")//支持多个域名端口
                    .WithMethods("GET", "POST", "PUT", "DELETE")//请求方法添加到策略
                    .WithHeaders("authorization");//标头添加到策略
                });

            });
            #endregion

            return services.BuildAspectInjectorProvider();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                #region Swagger
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    //之前是写死的
                    //c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
                    //c.RoutePrefix = "";//路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉

                    //根据版本名称倒序 遍历展示
                    typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                    {
                        c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
                    });
                });
                #endregion
            }
            else
            {
                // app.UseHsts(); 
                app.UseExceptionHandler();
            }

            app.UseHttpsRedirection();

            // app.UseMiddleware<JwtTokenAuth>();
            app.UseAuthentication();

            #region 跨域
            //跨域第二种方法，之间使用策略
            app.UseCors("LimitRequests");//将 CORS 中间件添加到 web 应用程序管线中, 以允许跨域请求。


            //跨域第一种版本，请要 services.AddCors();
            //    app.UseCors(options => options.WithOrigins("http://localhost:8021").AllowAnyHeader()
            //.AllowAnyMethod());
            #endregion

            app.UseMvc();
        }
    }
}
