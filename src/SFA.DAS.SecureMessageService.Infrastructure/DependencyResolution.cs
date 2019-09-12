using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.SecureMessageService.Core.Entities;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Core.IRepositories;
using SFA.DAS.SecureMessageService.Core.Services;
using SFA.DAS.SecureMessageService.Infrastructure.Repositories;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.SecureMessageService.Infrastructure
{
    public static class DependencyResolution
    {
        public static IServiceCollection SetupSecureMessageService
                (this IServiceCollection services, IConfiguration Configuration, IHostingEnvironment env)
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
                if (env.IsDevelopment())
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
                    var redis = ConnectionMultiplexer.Connect($"{redisConnectionString},DefaultDatabase=0");
                    services.AddDataProtection()
                        .SetApplicationName("das-sms-svc-web")
                        .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not create redis cache connection", e);
            }
            return services;
        }
    }
}
