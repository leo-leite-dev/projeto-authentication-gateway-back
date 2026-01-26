using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AuthService.Application.Abstractions.Security;
using AuthService.Application.Abstractions.Security.Jwt;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Security.Jwt;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly JwtClaimsFactory _claimsFactory;
    private readonly IJwtKeyProvider _keyProvider;

    public JwtTokenService(
        IOptions<JwtOptions> options,
        JwtClaimsFactory claimsFactory,
        IJwtKeyProvider keyProvider
    )
    {
        _options = options.Value;
        _claimsFactory = claimsFactory;
        _keyProvider = keyProvider;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = _claimsFactory.Create(user).ToList();

        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        var privateKey = _keyProvider.GetPrivateKey();

        var signingCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);

        var header = new JwtHeader(signingCredentials);
        header["kid"] = privateKey.KeyId;

        var payload = new JwtPayload(
            _options.Issuer,
            _options.Audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.Add(_options.AccessTokenLifetime)
        );

        var token = new JwtSecurityToken(header, payload);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}
