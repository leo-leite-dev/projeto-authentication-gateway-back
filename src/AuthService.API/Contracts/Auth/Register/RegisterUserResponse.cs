namespace AuthService.Api.Contracts.Auth.Register;

public sealed record RegisterUserResponse(
    Guid UserId,
    string Username,
    string Email,
    string Status
);
