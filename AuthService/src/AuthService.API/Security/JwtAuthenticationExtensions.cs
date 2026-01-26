using System.IdentityModel.Tokens.Jwt;
using AuthService.Application.Abstractions.Security;
using AuthService.Infrastructure.Security.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.API.Security;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.TryAddSingleton<IJwtKeyProvider, JwtKeyProvider>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "auth-service",

                    ValidateAudience = true,
                    ValidAudience = "api",

                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    {
                        var sp = services.BuildServiceProvider();
                        var keyProvider = sp.GetRequiredService<IJwtKeyProvider>();

                        return keyProvider.GetAllPublicKeys();
                    },

                    ClockSkew = TimeSpan.Zero,
                    ValidAlgorithms = new[] { SecurityAlgorithms.RsaSha256 },
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = ctx =>
                    {
                        if (ctx.SecurityToken is not JwtSecurityToken jwt)
                            throw new SecurityTokenException("Invalid token type");

                        if (jwt.Header.Alg != SecurityAlgorithms.RsaSha256)
                            throw new SecurityTokenInvalidAlgorithmException(
                                $"Invalid JWT alg: {jwt.Header.Alg}"
                            );

                        if (!jwt.Header.ContainsKey("kid"))
                            throw new SecurityTokenException("JWT sem kid");

                        return Task.CompletedTask;
                    },
                };
            });

        return services;
    }
}
