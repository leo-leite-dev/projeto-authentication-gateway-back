namespace AuthService.Application.UseCases.Users.GetCurrentUser;

public sealed record GetCurrentUserResult(Guid Id, string Username, string Email);
