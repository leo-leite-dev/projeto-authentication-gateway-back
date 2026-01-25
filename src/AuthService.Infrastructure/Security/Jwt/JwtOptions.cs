namespace AuthService.Infrastructure.Security.Jwt;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string SecretKey { get; init; } = null!;
    public int AccessTokenMinutes { get; init; }
}
