using AuthService.Domain.Entities;

namespace AuthService.Application.Security;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
