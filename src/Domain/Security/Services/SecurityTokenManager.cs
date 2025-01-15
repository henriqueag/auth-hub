using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AuthHub.Domain.Security.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SampleSecurityProvider.Security.Services;

public class SecurityTokenManager(IJwksManager jwksManager, IOptions<JwtOptions> jwtOptions) : ISecurityTokenManager
{
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var jwk = jwksManager.GetPrivateKey();

        var jwt = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddSeconds(jwtOptions.Value.LifetimeInSeconds),
            signingCredentials: new SigningCredentials(jwk, jwk.Alg)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}