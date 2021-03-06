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
using Blog.Core.AOP;

namespace Blog.Core
{
    /// <summary>
    /// 项目启动文件
    /// </summary>
    public class Startup1
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Startup1(IConfiguration configuration)
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
            services.AddScoped<ICacheKeyGenerator, DefaultCacheKeyGenerator>();
            #endregion

            #region AutoFac

            //实例化 AutoFac  容器   
            var builder = new ContainerBuilder();

            #region 注入切面
            builder.RegisterType<CacheOptions>();
            builder.RegisterType<BlogCacheAOP>();
            builder.RegisterType<AsyncExceptionHandlingInterceptor>();
            #endregion

            #region 注入Services/Repository

            // 手动注入
            // builder.RegisterType<AdvertisementServices>().As<IAdvertisementServices>();

            // 反射注入
            var assemblysServices = Assembly.Load("Blog.Core.Services");
            // 指定已扫描程序集中的类型注册为提供所有其实现的接口
            builder.RegisterAssemblyTypes(assemblysServices).AsImplementedInterfaces()
                .InstancePerLifetimeScope().EnableInterfaceInterceptors().InterceptedBy(typeof(BlogCacheAOP)); // 加入拦截器
            var assemblyRepository = Assembly.Load("Blog.Core.Repository");
            builder.RegisterAssemblyTypes(assemblyRepository).AsImplementedInterfaces();

            #endregion

            //将services填充到Autofac容器生成器中
            builder.Populate(services);

            //使用已进行的组件登记创建新容器
            var ApplicationContainer = builder.Build();

            #endregion

            return new AutofacServiceProvider(ApplicationContainer);//第三方IOC接管 core内置DI容器
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

            app.UseMvc();

            app.UseStaticFiles();   // 用于访问wwwroot下的文件
        }
    }
}
