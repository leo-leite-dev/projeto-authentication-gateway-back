using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using AuthService.Application.Security;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Security.Jwt;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly JwtClaimsFactory _claimsFactory;

    public JwtTokenService(IOptions<JwtOptions> options, JwtClaimsFactory claimsFactory)
    {
        _options = options.Value;
        _claimsFactory = claimsFactory;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = _claimsFactory.Create(user);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }
}
