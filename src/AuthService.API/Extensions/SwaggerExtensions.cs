using Microsoft.OpenApi.Models;

namespace AuthService.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Auth Service (Authentication Gateway)",
                    Version = "v1",
                    Description =
                        "Authentication Gateway responsible for login, session management "
                        + "and identity resolution. This service does NOT expose JWT or Bearer authentication.",
                }
            );
        });

        return services;
    }
}
