namespace AuthService.Api.Security.Cookies;

public sealed class AuthCookieService
{
    private const string AccessTokenCookieName = "access_token";
    private const string RefreshTokenCookieName = "refresh_token";

    private readonly IWebHostEnvironment _env;

    public AuthCookieService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public void AppendAccessToken(
        HttpResponse response,
        string accessToken,
        DateTimeOffset expiresAt
    )
    {
        var options = CookieOptionsFactory.CreateAccessTokenCookie(_env);
        options.Expires = expiresAt;

        response.Cookies.Append(AccessTokenCookieName, accessToken, options);
    }

    public void AppendRefreshToken(HttpResponse response, string refreshToken)
    {
        var options = CookieOptionsFactory.CreateRefreshTokenCookie(_env);

        response.Cookies.Append(RefreshTokenCookieName, refreshToken, options);
    }

    public void Clear(HttpResponse response)
    {
        var expired = CookieOptionsFactory.CreateExpiredCookie(_env);

        response.Cookies.Delete(AccessTokenCookieName, expired);
        response.Cookies.Delete(RefreshTokenCookieName, expired);
    }
}
