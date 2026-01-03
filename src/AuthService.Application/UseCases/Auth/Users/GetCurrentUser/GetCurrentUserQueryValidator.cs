using AuthService.Domain.Exceptions;

namespace AuthService.Application.UseCases.Users.GetCurrentUser;

public sealed class GetCurrentUserQueryValidator
{
    public void Validate(GetCurrentUserQuery query)
    {
        if (query.UserId == Guid.Empty)
            throw new UserException("Usuário inválido.");
    }
}
