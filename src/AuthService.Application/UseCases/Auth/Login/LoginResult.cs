namespace AuthService.Application.UseCases.Auth.Login;

public sealed record LoginResult(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt
);
