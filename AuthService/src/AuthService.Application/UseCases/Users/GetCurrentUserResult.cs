namespace AuthService.Application.UseCases.Users;

public sealed record GetCurrentUserResult(Guid Id, string Username, string Email, string Status);
