using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.SecureMessageService.Core.IRepositories;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Core.Services;
using SFA.DAS.SecureMessageService.Infrastructure.Repositories;

namespace SFA.DAS.SecureMessageService.Api.AppStart
{
    public static class ServiceRegistrationExtension
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IProtectionRepository, ProtectionRepository>();
            services.AddSingleton<ICacheRepository, CacheRepository>();
            services.AddSingleton<IDasDistributedCache, DasDistributedCache>();
            services.AddSingleton<IDasDataProtector, DasDataProtector>();
            services.AddSingleton<ISecureKeyRepository, SecureKeyRepository>();
        }
    }
}