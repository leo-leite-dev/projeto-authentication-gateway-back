namespace AuthService.Api.Contracts.Auth.Register;

public sealed record RegisterRequest(RegisterUserRequest User);

public sealed record RegisterUserRequest(string Username, string Email, string Password);
