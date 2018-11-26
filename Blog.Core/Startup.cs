﻿using System;
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

namespace Blog.Core
{
    /// <summary>
    /// 项目启动文件
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 项目配置
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v0.1.0",
                    Title = "Blog.Core API",
                    Description = "项目说明文档",
                    TermsOfService = "None",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact { Name = "Blog.Core", Email = "", Url = "" }
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

            #region 将 TService 中指定的类型的范围服务添加到实现
            services.AddScoped<ICacheManager, RedisCacheManager>();
            #endregion

            #region AspectCore
            services.AddTransient<IAdvertisementRepository, AdvertisementRepository>();
            services.AddTransient<IAdvertisementServices, AdvertisementServices>();

            services.AddTransient<ServicesCacheInterceptor>();
            services.ConfigureDynamicProxy(config =>
                {
                    config.Interceptors.AddServiced<ServicesCacheInterceptor>(Predicates.ForService("*Services*"));
                    config.NonAspectPredicates.AddMethod("Add*");
                    config.NonAspectPredicates.AddMethod("Update*");
                    config.NonAspectPredicates.AddMethod("Delete*");
                }
            );

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

            #endregion
            return services.BuildAspectInjectorProvider();
            // return new AutofacServiceProvider(ApplicationContainer);//第三方IOC接管 core内置DI容器
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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
                    /* 如果想直接在域名的根目录直接加载 swagger 比如访问：localhost:8001 就能访问，可以这样设置：
                     * c.RoutePrefix = "";//路径配置，设置为空，表示直接访问该文件
                     */
                    // c.RoutePrefix = "";
                });
                #endregion
            }
            else
            {
                // app.UseHsts(); 
                app.UseExceptionHandler();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<JwtTokenAuth>();

            app.UseCors("LimitRequests");

            app.UseMvc();

            app.UseStaticFiles();   // 用于访问wwwroot下的文件
        }
    }
}
