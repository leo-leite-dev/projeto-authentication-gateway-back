using AuthService.Domain.Entities;

namespace AuthService.Application.Abstractions.Security;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
