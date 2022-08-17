using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Logging;
using SFA.DAS.SecureMessageService.Web.AppStart;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;
IdentityModelEventSource.ShowPII = false;

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddServices(configuration);

builder.Services.AddOptions();

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(configuration);
builder.Services.AddHealthChecks();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
.AddControllersAsServices();;

builder.Services.AddApplicationInsightsTelemetry(configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

builder.Services.AddDistributedCache(configuration, environment);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
} else {
    app.UseDeveloperExceptionPage();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Xss-Protection", "1");
    await next();
});

app.UsePathBase("/messages");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    if (context.Response.Headers.ContainsKey("X-Frame-Options"))
    {
        context.Response.Headers.Remove("X-Frame-Options");
    }

    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");

    await next();

    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        //Re-execute the request so the user gets the error page
        var originalPath = context.Request.Path.Value;
        context.Items["originalPath"] = originalPath;
        context.Request.Path = "/error/404";
        await next();
    }
});

app.UseRouting();
app.UseAuthentication();
app.UseHealthChecks("/health");
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
