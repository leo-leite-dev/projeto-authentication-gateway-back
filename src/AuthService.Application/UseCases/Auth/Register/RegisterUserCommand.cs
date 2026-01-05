namespace AuthService.Application.UseCases.Auth.Register;

public sealed record RegisterUserCommand(string Username, string Email, string Password);
