using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Domain.Entities;

namespace AuthService.Application.Abstractions.Security.Jwt;

public sealed class JwtClaimsFactory
{
    public IEnumerable<Claim> Create(User user)
    {
        return new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim("username", user.Username.Value),
            new Claim("tv", user.TokenVersion.ToString()),
        };
    }
}
