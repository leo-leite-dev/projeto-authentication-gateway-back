namespace AuthService.Api.Contracts.Auth;

public sealed record LoginResponse(string AccessToken, DateTime AccessTokenExpiresAt);
