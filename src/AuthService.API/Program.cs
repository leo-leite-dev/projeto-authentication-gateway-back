using AuthService.Api.Extensions;
using AuthService.Api.Filters;
using AuthService.Api.Middlewares;
using AuthService.Api.Security;
using AuthService.Api.Security.Cookies;
using AuthService.Application.Abstractions.Security;
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

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocumentation();

builder.Services.AddScoped<AuthCookieService>();
builder.Services.AddHttpClient<GatewayForwarder>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "FrontEnd",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.SameAsRequest;
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors("FrontEnd");

app.UseCookiePolicy();

app.MapControllers();

app.Run();
