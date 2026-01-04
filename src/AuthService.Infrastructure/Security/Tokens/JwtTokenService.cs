using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AuthService.Application.Abstractions.Security;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Security.Tokens;

public sealed class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateToken(User user, TokenType tokenType)
    {
        var claims = tokenType switch
        {
            TokenType.AccessToken => ClaimsFactory.CreateForAccessToken(user),
            TokenType.RefreshToken => ClaimsFactory.CreateForRefreshToken(user),
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType)),
        };

        var expires =
            tokenType == TokenType.AccessToken
                ? DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes)
                : DateTime.UtcNow.AddDays(_options.RefreshTokenDays);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
