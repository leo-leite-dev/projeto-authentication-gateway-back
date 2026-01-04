using Microsoft.AspNetCore.Http;

namespace AuthService.Infrastructure.Security.Cookies;

public static class CookieOptionsFactory
{
    public static CookieOptions CreateAccessTokenCookie()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            IsEssential = true,
        };
    }

    public static CookieOptions CreateRefreshTokenCookie()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            IsEssential = true,
        };
    }

    public static CookieOptions CreateExpiredCookie()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
            IsEssential = true,
        };
    }
}
