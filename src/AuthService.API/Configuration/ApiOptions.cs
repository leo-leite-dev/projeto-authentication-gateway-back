namespace AuthService.Api.Configuration;

public sealed class ApiOptions
{
    public const string SectionName = "Api";

    public string AccessTokenCookieName { get; init; } = "access_token";
    public string RefreshTokenCookieName { get; init; } = "refresh_token";
}
