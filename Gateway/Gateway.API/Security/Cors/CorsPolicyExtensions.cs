namespace Gateway.Api.Security.Cors;

public static class CorsPolicyExtensions
{
    public const string DefaultPolicy = "DefaultCorsPolicy";

    public static IServiceCollection AddStrongCors(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        var options = config.GetSection("Cors").Get<CorsPolicyOptions>();

        services.AddCors(cors =>
        {
            cors.AddPolicy(
                DefaultPolicy,
                policy =>
                {
                    policy.WithOrigins(options.AllowedOrigins).AllowAnyHeader().AllowAnyMethod();

                    if (options.AllowCredentials)
                        policy.AllowCredentials();
                }
            );
        });

        return services;
    }
}
