using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SampleSecurityProvider.Security.Options;

namespace SampleSecurityProvider.Security.Services;

public class JwksManager(IOptions<JwtOptions> jwtOptions) : IJwksManager
{
    public JsonWebKeySet GetJwks() => new() { Keys = { GetPublicKey() } };
    
    public JsonWebKey GetPublicKey()
    {
        var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(GetSecurityKey(jwtOptions.Value.PublicKeyPath));
        SetCommonProperties(jwk);
        return jwk;
    }
    
    public JsonWebKey GetPrivateKey()
    {
        var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(GetSecurityKey(jwtOptions.Value.PrivateKeyPath));
        SetCommonProperties(jwk);
        return jwk;
    }
    
    private static RsaSecurityKey GetSecurityKey(string path)
    {
        var rsa = RSA.Create();
        rsa.FromXmlString(File.ReadAllText(path));
        return new RsaSecurityKey(rsa)
        {
            KeyId = "adaa6fb9c6664fc59c02259451e97465"
        };
    }

    private static void SetCommonProperties(JsonWebKey jwk)
    {
        jwk.Alg = SecurityAlgorithms.RsaSha256;
        jwk.Use = "sig";
    }
}