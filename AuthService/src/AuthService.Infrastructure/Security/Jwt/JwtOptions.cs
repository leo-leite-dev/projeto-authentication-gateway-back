namespace AuthService.Infrastructure.Security.Jwt;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public TimeSpan AccessTokenLifetime { get; init; }
}
