using AuthService.Api.Extensions;
using AuthService.Api.Filters;
using AuthService.Api.Middlewares;
using AuthService.Api.Security.Cookies;
using AuthService.Application.DependencyInjection;
using AuthService.Infrastructure.DependencyInjection;
using AuthService.Infrastructure.Gateway.Forwarding;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddApplication().AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<AuthCookieService>();

builder.Services.AddHttpClient<GatewayForwarder>();

builder.Services.AddSwaggerDocumentation();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCookiePolicy();

app.MapControllers();

app.Run();
