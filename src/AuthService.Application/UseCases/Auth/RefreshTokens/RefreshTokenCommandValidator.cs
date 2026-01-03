using AuthService.Domain.Exceptions;

namespace AuthService.Application.UseCases.Auth.RefreshTokens;

public sealed class RefreshTokenCommandValidator
{
    public void Validate(RefreshTokenCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            throw new UserException("Refresh token é obrigatório.");
    }
}
