using Microsoft.IdentityModel.Tokens;

namespace AuthService.Application.Abstractions.Security;

public interface IJwtKeyProvider
{
    SecurityKey GetPrivateKey();
    SecurityKey GetPublicKey(string kid);
    IEnumerable<SecurityKey> GetAllPublicKeys();
}
