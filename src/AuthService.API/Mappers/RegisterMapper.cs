using AuthService.Api.Contracts.Auth.Register;
using AuthService.Application.UseCases.Auth.Register;

namespace AuthService.Api.Mappers;

public static class RegisterRequestMapper
{
    public static RegisterUserCommand ToCommand(this RegisterRequest request) =>
        new(
            request.User.Username.Trim(),
            request.User.Email.Trim().ToLowerInvariant(),
            request.User.Password
        );
}
