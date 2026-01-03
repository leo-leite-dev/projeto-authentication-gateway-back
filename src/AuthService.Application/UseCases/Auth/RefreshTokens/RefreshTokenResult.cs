namespace AuthService.Application.UseCases.Auth.RefreshTokens;

public sealed record RefreshTokenResult(string AccessToken, DateTime AccessTokenExpiresAt);
