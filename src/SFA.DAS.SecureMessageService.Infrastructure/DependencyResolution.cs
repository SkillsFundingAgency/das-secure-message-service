using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.SecureMessageService.Core.Entities;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Core.IRepositories;
using SFA.DAS.SecureMessageService.Core.Services;
using SFA.DAS.SecureMessageService.Infrastructure.Repositories;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.SecureMessageService.Infrastructure
{
    [ExcludeFromCodeCoverage]
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
                        options.Configuration = $"{redisConnectionString},DefaultDatabase=0";
                    });
                    var redis = ConnectionMultiplexer.Connect($"{redisConnectionString},DefaultDatabase=0");
                    services.AddDataProtection()
                        .SetApplicationName("das-tools-service")
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
