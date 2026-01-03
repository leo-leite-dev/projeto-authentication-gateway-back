using AuthService.Application.UseCases.Auth.Login;
using AuthService.Application.UseCases.Auth.Logout;
using AuthService.Application.UseCases.Auth.RefreshTokens;
using AuthService.Application.UseCases.Users.GetCurrentUser;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application.DependencyInjection;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Login
        services.AddScoped<LoginUseCase>();
        services.AddScoped<LoginCommandValidator>();

        // Refresh token
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<RefreshTokenCommandValidator>();

        // Logout
        services.AddScoped<LogoutUseCase>();
        services.AddScoped<LogoutCommandValidator>();

        // User
        services.AddScoped<GetCurrentUserQueryValidator>();

        return services;
    }
}
