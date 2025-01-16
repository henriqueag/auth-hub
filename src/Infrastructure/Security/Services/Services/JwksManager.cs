using System.Collections.Concurrent;
using System.Security.Cryptography;
using AuthHub.Domain.Security.Services;
using Microsoft.IdentityModel.Tokens;

namespace AuthHub.Infrastructure.Security.Services.Services;

public class JwksManager : IJwksManager
{
    private const string PublicKeyName = "PublicKey";
    private const string PrivateKeyName = "PrivateKey";

    private static Lazy<ConcurrentDictionary<string, string>> RsaKeys => new(LoadRsaKeys);
    
    public JsonWebKeySet GetJwks() => new() { Keys = { GetPublicKey() } };
    
    public JsonWebKey GetPublicKey()
    {
        var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(GetSecurityKey(PublicKeyName));
        SetCommonProperties(jwk);
        return jwk;
    }
    
    public JsonWebKey GetPrivateKey()
    {
        var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(GetSecurityKey(PrivateKeyName));
        SetCommonProperties(jwk);
        return jwk;
    }
    
    private static RsaSecurityKey GetSecurityKey(string name)
    {
        var rsa = RSA.Create();
        rsa.FromXmlString(RsaKeys.Value[name]);
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

    private static ConcurrentDictionary<string, string> LoadRsaKeys()
    {
        var assembly = typeof(JwksManager).Assembly;
        var keys = new ConcurrentDictionary<string, string>();
        
        foreach (var name in assembly.GetManifestResourceNames())
        {
            if (name.Contains(PublicKeyName))
            {
                var stream = assembly.GetManifestResourceStream(name)!;
                using var streamReader = new StreamReader(stream);
                keys[PublicKeyName] = streamReader.ReadToEnd();
            }

            if (name.Contains(PrivateKeyName))
            {
                var stream = assembly.GetManifestResourceStream(name)!;
                using var streamReader = new StreamReader(stream);
                keys[PrivateKeyName] = streamReader.ReadToEnd();
            }
        }

        return keys;
    }
}