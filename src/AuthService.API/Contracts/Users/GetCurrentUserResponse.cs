namespace AuthService.Api.Contracts.Users;

public sealed record GetCurrentUserResponse(Guid Id, string Username, string Email, string Status);
