namespace AuthService.Api.Security.Cookies;

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

    public void AppendRefreshToken(HttpResponse response, string refreshToken)
    {
        var options = CookieOptionsFactory.CreateRefreshTokenCookie();

        response.Cookies.Append(RefreshTokenCookieName, refreshToken, options);
    }

    public void RemoveTokens(HttpResponse response)
    {
        var expired = CookieOptionsFactory.CreateExpiredCookie();

        response.Cookies.Delete(AccessTokenCookieName, expired);
        response.Cookies.Delete(RefreshTokenCookieName, expired);
    }
}
