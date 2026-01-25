namespace AuthService.Api.Contracts.Auth.Login;

public sealed record AuthUserDto(
    Guid UserId,
    string Username,
    string Email,
    string Status,
    string Token
);
