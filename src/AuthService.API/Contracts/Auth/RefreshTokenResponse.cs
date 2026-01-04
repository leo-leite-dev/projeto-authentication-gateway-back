namespace AuthService.Api.Contracts.Auth;

public sealed record RefreshTokenResponse(string AccessToken, DateTime AccessTokenExpiresAt);
