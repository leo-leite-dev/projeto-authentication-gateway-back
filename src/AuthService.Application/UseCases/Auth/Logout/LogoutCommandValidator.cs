using AuthService.Domain.Exceptions;

namespace AuthService.Application.UseCases.Auth.Logout;

public sealed class LogoutCommandValidator
{
    public void Validate(LogoutCommand command)
    {
        if (command.UserId == Guid.Empty)
            throw new UserException("Usuário inválido.");
    }
}
