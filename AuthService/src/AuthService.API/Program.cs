using AuthService.Api.Extensions;
using AuthService.Api.Filters;
using AuthService.Api.Middlewares;
using AuthService.Api.Security;
using AuthService.Api.Security.Cookies;
using AuthService.Application.Abstractions.Security;
using AuthService.Application.DependencyInjection;
using AuthService.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddApplication().AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocumentation();

builder.Services.AddScoped<AuthCookieService>();


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


app.UseCookiePolicy();

app.MapControllers();

app.MapGet("/health", () => Results.Ok("AuthService OK"));

app.Run();
