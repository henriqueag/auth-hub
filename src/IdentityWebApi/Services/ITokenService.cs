using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityWebApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace IdentityWebApi.Services;

public interface ITokenService
{
    object CreateToken(ApplicationUser user);
}

public class TokenService : ITokenService
{
    public object CreateToken(ApplicationUser user)
    {
        var expiration = DateTime.UtcNow.AddMinutes(10);

        var token = CreateJwtToken(
            CreateClaims(user),
            CreateSigningCredentials(),
            expiration
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        return new
        {
            Token = tokenHandler.WriteToken(token),
            Expiration = expiration
        };
    }

    private JwtSecurityToken CreateJwtToken(Claim[] claims, SigningCredentials credentials, DateTime expiration) =>
        new(
            issuer: "http://localhost:5142",
            audience: "http://localhost:5142",
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

    private Claim[] CreateClaims(ApplicationUser user) =>
        new[] 
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

    private SigningCredentials CreateSigningCredentials() =>
        new(
            key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes("b2c1c619fc6b404598349fded6b93ee7")), 
            algorithm: SecurityAlgorithms.HmacSha256);
}
