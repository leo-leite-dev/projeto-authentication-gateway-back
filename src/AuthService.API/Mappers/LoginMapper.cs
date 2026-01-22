using AuthService.Api.Contracts.Auth.Login;
using AuthService.Application.UseCases.Auth.Login;

namespace AuthService.Api.Mappers;

public static class LoginMapper
{
    public static LoginCommand ToCommand(this LoginRequest request) =>
        new(request.User.Email.Trim().ToLowerInvariant(), request.User.Password);
}
