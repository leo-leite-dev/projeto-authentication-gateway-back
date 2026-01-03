using AuthService.Domain.Entities;
using AuthService.Domain.Enums;

namespace AuthService.Application.Abstractions.Security;

public interface ITokenService
{
    string GenerateToken(User user, TokenType tokenType);
}
