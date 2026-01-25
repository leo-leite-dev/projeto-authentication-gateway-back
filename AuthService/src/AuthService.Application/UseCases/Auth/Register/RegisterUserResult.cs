namespace AuthService.Application.UseCases.Auth.Register;

public sealed record RegisterUserResult(Guid UserId, string Username, string Email, string Status);
