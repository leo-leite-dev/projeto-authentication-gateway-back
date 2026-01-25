namespace AuthService.Api.Extensions;

public static class CookieExtensions
{
    public static IApplicationBuilder UseSecureCookies(this IApplicationBuilder app)
    {
        app.UseCookiePolicy();
        return app;
    }
}
