namespace AuthService.Api.Contracts.Auth.Login;

public sealed record LoginResponse(Guid UserId, string Username, string Email, string Status);
