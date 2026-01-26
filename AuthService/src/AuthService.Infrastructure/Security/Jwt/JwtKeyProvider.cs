using System.Security.Cryptography;
using AuthService.Application.Abstractions.Security;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Security.Jwt;

public sealed class JwtKeyProvider : IJwtKeyProvider
{
    private readonly IReadOnlyDictionary<string, RsaSecurityKey> _privateKeys;
    private readonly IReadOnlyDictionary<string, RsaSecurityKey> _publicKeys;
    private readonly string _activeKeyId;

    public JwtKeyProvider()
    {
        var privateKeys = new Dictionary<string, RsaSecurityKey>();
        var publicKeys = new Dictionary<string, RsaSecurityKey>();

        var keysPath = Path.Combine(AppContext.BaseDirectory, "Keys");

        if (!Directory.Exists(keysPath))
            throw new InvalidOperationException($"Pasta de chaves não encontrada: {keysPath}");

        var pemFiles = Directory.GetFiles(keysPath, "*-private.pem");

        foreach (var file in pemFiles)
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText(file));

            var keyId = Path.GetFileNameWithoutExtension(file);

            privateKeys[keyId] = new RsaSecurityKey(rsa) { KeyId = keyId };

            var pubFile = file.Replace("-private.pem", "-public.pem");

            if (!File.Exists(pubFile))
                throw new InvalidOperationException($"Arquivo público não encontrado: {pubFile}");

            var pubRsa = RSA.Create();
            pubRsa.ImportFromPem(File.ReadAllText(pubFile));

            publicKeys[keyId] = new RsaSecurityKey(pubRsa) { KeyId = keyId };
        }

        if (!privateKeys.Any())
            throw new InvalidOperationException("Nenhuma chave RSA encontrada na pasta Keys/");

        var activeKeyFile = Path.Combine(keysPath, "active.key");
        if (!File.Exists(activeKeyFile))
            throw new InvalidOperationException("Arquivo active.key não encontrado na pasta Keys/");

        _activeKeyId = File.ReadAllText(activeKeyFile).Trim();

        if (!privateKeys.ContainsKey(_activeKeyId))
            throw new InvalidOperationException(
                $"Active key '{_activeKeyId}' não encontrada entre as chaves privadas"
            );

        _privateKeys = privateKeys;
        _publicKeys = publicKeys;
    }

    public SecurityKey GetPrivateKey() => _privateKeys[_activeKeyId];

    public SecurityKey GetPublicKey(string kid) =>
        _publicKeys.TryGetValue(kid, out var key)
            ? key
            : throw new KeyNotFoundException($"Chave com kid '{kid}' não encontrada");

    public IEnumerable<SecurityKey> GetAllPublicKeys() => _publicKeys.Values;
}
