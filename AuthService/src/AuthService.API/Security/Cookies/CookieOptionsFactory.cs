namespace AuthService.Api.Security.Cookies;

public static class CookieOptionsFactory
{
    public static CookieOptions CreateAccessTokenCookie(IWebHostEnvironment env)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !env.IsDevelopment(), // ✅
            SameSite = env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict,
            Path = "/",
            IsEssential = true,
        };
    }

    public static CookieOptions CreateRefreshTokenCookie(IWebHostEnvironment env)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !env.IsDevelopment(),
            SameSite = env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict,
            Path = "/",
            IsEssential = true,
        };
    }

    public static CookieOptions CreateExpiredCookie(IWebHostEnvironment env)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !env.IsDevelopment(),
            SameSite = env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
            IsEssential = true,
        };
    }
}
