namespace AuthService.API.Security;

public static class AntiforgeryExtensions
{
    public static IServiceCollection AddCustomAntiforgery(
        this IServiceCollection services,
        IWebHostEnvironment env
    )
    {
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.Name = "XSRF-TOKEN";
            options.Cookie.HttpOnly = false;

            if (env.IsDevelopment())
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SameSite = SameSiteMode.Lax;
            }
            else
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            }

            options.FormFieldName = "__RequestVerificationToken";
        });

        return services;
    }
}
