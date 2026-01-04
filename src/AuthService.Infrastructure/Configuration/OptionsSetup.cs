using AuthService.Infrastructure.Security.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.Configuration;

public static class OptionsSetup
{
    public static IServiceCollection AddInfrastructureOptions(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.Issuer)
                    && !string.IsNullOrWhiteSpace(options.Audience)
                    && !string.IsNullOrWhiteSpace(options.SecretKey)
                    && options.AccessTokenMinutes > 0
                    && options.RefreshTokenDays > 0,
                "Configuração de JWT inválida."
            )
            .ValidateOnStart();

        return services;
    }
}
