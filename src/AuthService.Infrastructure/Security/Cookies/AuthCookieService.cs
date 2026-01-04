using Microsoft.AspNetCore.Http;

namespace AuthService.Infrastructure.Security.Cookies;

public sealed class AuthCookieService
{
    private const string AccessTokenCookieName = "access_token";
    private const string RefreshTokenCookieName = "refresh_token";

    public void AppendAccessToken(
        HttpResponse response,
        string accessToken,
        DateTimeOffset expiresAt
    )
    {
        var options = CookieOptionsFactory.CreateAccessTokenCookie();
        options.Expires = expiresAt;

        response.Cookies.Append(AccessTokenCookieName, accessToken, options);
    }

    public void AppendRefreshToken(
        HttpResponse response,
        string refreshToken,
        DateTimeOffset expiresAt
    )
    {
        var options = CookieOptionsFactory.CreateRefreshTokenCookie();
        options.Expires = expiresAt;

        response.Cookies.Append(RefreshTokenCookieName, refreshToken, options);
    }

    public void RemoveTokens(HttpResponse response)
    {
        response.Cookies.Delete(AccessTokenCookieName, CookieOptionsFactory.CreateExpiredCookie());

        response.Cookies.Delete(RefreshTokenCookieName, CookieOptionsFactory.CreateExpiredCookie());
    }
}
