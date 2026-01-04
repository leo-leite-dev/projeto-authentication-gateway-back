using System.Security.Claims;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Security.Tokens;

public static class ClaimsFactory
{
    public static IEnumerable<Claim> CreateForAccessToken(User user)
    {
        return new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username.Value),
            new Claim(ClaimTypes.Email, user.Email.Value),
            new Claim("status", user.Status.ToString()),
        };
    }

    public static IEnumerable<Claim> CreateForRefreshToken(User user)
    {
        return new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("typ", "refresh"),
        };
    }
}
