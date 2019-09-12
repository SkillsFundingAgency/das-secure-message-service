using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using SFA.DAS.SecureMessageService.Core.Entities;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Core.Services;
using SFA.DAS.SecureMessageService.Core.IRepositories;
using SFA.DAS.SecureMessageService.Infrastructure.Repositories;

namespace SFA.DAS.SecureMessageService.Api
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SharedConfig>(Configuration);
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IProtectionRepository, ProtectionRepository>();
            services.AddSingleton<ICacheRepository, CacheRepository>();
            services.AddSingleton<IDasDistributedCache, DasDistributedCache>();
            services.AddSingleton<IDasDataProtector, DasDataProtector>();
            services.AddSingleton<ISecureKeyRepository, SecureKeyRepository>();

            try
            {
                if (_env.IsDevelopment())
                {
                    services.AddDistributedMemoryCache();
                }
                else
                {
                    var redisConnectionString = Configuration["RedisConnectionString"];

                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = $"{redisConnectionString},DefaultDatabase=1";
                    });
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not create redis cache connection", e);
            }

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "DAS Secure Message Service API", Version = "v1" });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // Enable app insights logging
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Warning);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DAS Secure Message Service API");
                });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
