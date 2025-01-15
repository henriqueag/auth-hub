using Microsoft.IdentityModel.Tokens;

namespace AuthHub.Domain.Security.Services;

public interface IJwksManager
{
    JsonWebKeySet GetJwks();
    JsonWebKey GetPublicKey();
    JsonWebKey GetPrivateKey();
}