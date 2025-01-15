using Microsoft.IdentityModel.Tokens;

namespace SampleSecurityProvider.Security.Services;

public interface IJwksManager
{
    JsonWebKeySet GetJwks();
    JsonWebKey GetPublicKey();
    JsonWebKey GetPrivateKey();
}