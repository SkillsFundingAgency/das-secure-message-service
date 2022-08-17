using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using SFA.DAS.SecureMessageService.Api.AppStart;
using SFA.DAS.SecureMessageService.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Services.AddServices(configuration);
builder.Services.AddApplicationInsightsTelemetry(configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

if (!environment.IsDevelopment())
{
    builder.Services.AddAuthentication(auth => { auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
        .AddJwtBearer(auth =>
        {
            auth.Authority =
                $"https://login.microsoftonline.com/{configuration["AzureAd:Tenant"]}";
            auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidAudiences = configuration["AzureAd:Identifier"].Split(',')
            };
        });

    builder.Services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(ApiConstants.AuthorizationPolicyName, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(ApiConstants.AuthorizationRequiredRoleName);
        });
    });
}

builder.Services.AddHealthChecks();

builder.Services.AddDistributedCache(configuration, environment);

builder.Services.AddControllers(options =>
{
    if (!environment.IsDevelopment())
    {
        options.Filters.Add(new AuthorizeFilter(ApiConstants.AuthorizationPolicyName));
    }

});

builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = ApiConstants.ApiName, Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
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

app.UsePathBase(ApiConstants.PathBase);
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

app.UseSwagger();
app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"{ApiConstants.PathBase}/swagger/v1/swagger.json", ApiConstants.ApiName);
    });

app.UseRouting();

app.UseAuthorization();

app.UseAuthentication();
app.UseHealthChecks("/health");
app.UseHttpsRedirection();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
