namespace AuthService.Api.Contracts.Auth.Login;

public sealed record LoginRequest(LoginUserRequest User);

public sealed record LoginUserRequest(string Email, string Password);
