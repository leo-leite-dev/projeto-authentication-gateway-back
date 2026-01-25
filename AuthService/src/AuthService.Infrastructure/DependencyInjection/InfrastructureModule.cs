using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Security;
using AuthService.Application.Abstractions.Time;
using AuthService.Application.Security;
using AuthService.Infrastructure.Persistence.Context;
using AuthService.Infrastructure.Persistence.Repositories;
using AuthService.Infrastructure.Security.Hashing;
using AuthService.Infrastructure.Security.Jwt;
using AuthService.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.DependencyInjection;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<JwtClaimsFactory>();

        return services;
    }
}
