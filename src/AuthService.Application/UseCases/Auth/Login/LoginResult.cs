namespace AuthService.Application.UseCases.Auth.Login;

public sealed record LoginResult(
    Guid UserId,
    string Username,
    string Email,
    string Status,
    string AccessToken
);
