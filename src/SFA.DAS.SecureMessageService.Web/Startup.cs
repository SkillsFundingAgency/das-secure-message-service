using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.HttpOverrides;
using SFA.DAS.ToolService.Authentication.ServiceCollectionExtensions;
using SFA.DAS.ToolService.Authentication.Entities;
using SFA.DAS.SecureMessageService.Infrastructure;

namespace SFA.DAS.SecureMessageService.Web
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authenticationOptions = Configuration.GetSection("Authentication");

            services.Configure<AuthenticationConfigurationEntity>(authenticationOptions);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.SetupSecureMessageService(Configuration, _env);

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = ".AspNet.SharedCookie";
                options.Cookie.Path = "/";
            });

            services.AddAntiforgery(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddHealthChecks();

            services.AddAuth0(authenticationOptions.Get<AuthenticationConfigurationEntity>());

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
        {

            app.UseForwardedHeaders();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Xss-Protection", "1");
                await next();
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Headers.ContainsKey("X-Original-Host"))
                {
                    var originalHost = context.Request.Headers["X-Original-Host"];
                    logger.LogInformation($"Retrieving X-Original-Host value {originalHost}");
                    context.Request.Headers.Add("Host", originalHost);
                }
                await next.Invoke();
            });

            if (env.IsDevelopment())
            {
                logger.LogInformation($"App is running in development mode: {env.EnvironmentName}");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                logger.LogInformation($"App is running in production mode: {env.EnvironmentName}");
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UsePathBase("/messages");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseHealthChecks("/health");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
