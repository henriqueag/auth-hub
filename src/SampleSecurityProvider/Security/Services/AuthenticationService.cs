using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SampleSecurityProvider.Models;
using SampleSecurityProvider.Security.Options;
using SampleSecurityProvider.Security.ValueObjects;
using SecurityToken = SampleSecurityProvider.Security.ValueObjects.SecurityToken;

namespace SampleSecurityProvider.Security.Services;

public class AuthenticationService(UserManager<ApplicationUser> userManager, ISecurityTokenManager tokenManager, IOptions<JwtOptions> jwtOptions) : IAuthenticationService
{
    public async Task<SecurityToken> CreateTokenAsync(ApplicationUser user, string password)
    {
        var passwordMatch = await userManager.CheckPasswordAsync(user, password);
        if (!passwordMatch)
        {
            throw new Exception();
        }

        return await CreateTokenCore(user);
    }

    public async Task<SecurityToken> RefreshTokenAsync(ApplicationUser user, string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new Exception();
        }

        var refreshTokenJson = await userManager.GetAuthenticationTokenAsync(user, "Application", "RefreshToken");
        if (string.IsNullOrEmpty(refreshTokenJson))
        {
            throw new Exception();
        }

        var savedRefreshToken = RefreshToken.Deserialize(refreshTokenJson);
        if (refreshToken != savedRefreshToken.Value || savedRefreshToken.Lifetime.Ticks > DateTime.UtcNow.Ticks)
        {
            throw new Exception();
        }
        
        return await CreateTokenCore(user);
    }

    private async Task<SecurityToken> CreateTokenCore(ApplicationUser user)
    {
        var accessToken = tokenManager.GenerateAccessToken(await CreateClaimsAsync(user));
        var refreshToken = tokenManager.GenerateRefreshToken();

        await StoreRefreshTokenAsync(user, refreshToken);
        
        return new SecurityToken(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            TokenType: JwtBearerDefaults.AuthenticationScheme,
            ExpiresIn: jwtOptions.Value.LifetimeInSeconds
        );
    }
    
    private async Task<List<Claim>> CreateClaimsAsync(ApplicationUser user)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow.ToUniversalTime()).ToString(), ClaimValueTypes.Integer64),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        ];

        var roles = await userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    private async Task StoreRefreshTokenAsync(ApplicationUser user, string refreshTokenValue)
    {
        var refreshToken = new RefreshToken(refreshTokenValue, TimeSpan.FromSeconds(jwtOptions.Value.RefreshTokenLifetimeInSeconds));
        await userManager.SetAuthenticationTokenAsync(user, "Application", "RefreshToken", refreshToken.Serialize());
    }
}