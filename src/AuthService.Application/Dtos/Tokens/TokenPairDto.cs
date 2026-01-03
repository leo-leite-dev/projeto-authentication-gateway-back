namespace AuthService.Application.DTOs.Tokens;

public sealed record TokenPairDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt
);
