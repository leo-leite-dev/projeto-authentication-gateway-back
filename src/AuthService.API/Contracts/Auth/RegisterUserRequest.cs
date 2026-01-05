namespace AuthService.Api.Contracts.Auth;

public sealed record RegisterUserRequest(string Username, string Email, string Password);
