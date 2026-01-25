using AuthService.Domain.Exceptions;

namespace AuthService.Application.UseCases.Auth.Login;

public sealed class LoginCommandValidator
{
    public void Validate(LoginCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Login))
            throw new UserException("Login é obrigatório.");

        if (string.IsNullOrWhiteSpace(command.Password))
            throw new UserException("Senha é obrigatória.");

        if (command.Password.Length < 6)
            throw new UserException("Senha deve possuir ao menos 6 caracteres.");
    }
}
