using AuthService.Application.Common.Exceptions;

namespace AuthService.Application.UseCases.Auth.Register;

public sealed class RegisterUserCommandValidator
{
    public void Validate(RegisterUserCommand command)
    {
        if (command is null)
            throw new ValidationException("Comando inválido.");

        if (string.IsNullOrWhiteSpace(command.Username))
            throw new ValidationException("Username é obrigatório.");

        if (string.IsNullOrWhiteSpace(command.Email))
            throw new ValidationException("Email é obrigatório.");

        if (string.IsNullOrWhiteSpace(command.Password))
            throw new ValidationException("Senha é obrigatória.");
    }
}
