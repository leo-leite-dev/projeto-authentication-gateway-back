using AuthService.Application.UseCases.Auth.Login;
using AuthService.Application.UseCases.Auth.Logout;
using AuthService.Application.UseCases.Auth.RefreshTokens;
using AuthService.Application.UseCases.Auth.Register;
using AuthService.Application.UseCases.Users;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application.DependencyInjection;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<LoginUseCase>();
        services.AddScoped<LoginCommandValidator>();

        services.AddScoped<RefreshTokenUseCase>();

        services.AddScoped<LogoutUseCase>();

        services.AddScoped<RegisterUserUseCase>();
        services.AddScoped<RegisterUserCommandValidator>();

        services.AddScoped<GetCurrentUserQueryValidator>();

        return services;
    }
}
