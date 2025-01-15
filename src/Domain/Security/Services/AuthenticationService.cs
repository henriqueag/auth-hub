using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthHub.Domain.Security.Options;
using AuthHub.Domain.Users.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecurityToken = SampleSecurityProvider.Security.ValueObjects.SecurityToken;

namespace SampleSecurityProvider.Security.Services;

public class AuthenticationService(
    UserManager<User> userManager,
    ISecurityTokenManager tokenManager,
    IRefreshTokenRepository refreshTokenRepository,
    IOptions<JwtOptions> jwtOptions,
    ILogger<AuthenticationService> logger
) : IAuthenticationService
{
    public async Task<SecurityToken> CreateTokenAsync(User user, string password)
    {
        logger.LogInformation("Iniciando a criação de token para o usuário {UserId}.", user.Id);

        ThrowIfInactiveUser(user);
        
        var passwordMatch = await userManager.CheckPasswordAsync(user, password);
        if (!passwordMatch)
        {
            throw new ProblemDetailsException(
                "security.authentication-service.invalid-credentials",
                "As credenciais fornecidas estão incorretas.",
                StatusCodes.Status401Unauthorized
            );
        }

        return await CreateTokenCore(user);
    }

    public async Task<SecurityToken> RefreshTokenAsync(string refreshToken)
    {
        logger.LogInformation("Iniciando o processo de atualização de token para o refresh token fornecido.");
        
        var savedRefreshToken = await refreshTokenRepository.GetAsync(refreshToken);
        if (savedRefreshToken is null)
        {
            throw new ProblemDetailsException(
                "security.authentication-service.refresh-token-not-found",
                "O token de atualização fornecido não foi encontrado.",
                StatusCodes.Status404NotFound);
        }

        if (savedRefreshToken.IsExpired(TimeSpan.FromSeconds(jwtOptions.Value.RefreshTokenLifetimeInSeconds)) || savedRefreshToken.IsRevoked)
        {
            throw new ProblemDetailsException(
                "security.authentication-service.refresh-token-expired-or-revoked",
                "O token de atualização fornecido está expirado ou foi revogado. Solicite um novo token de acesso.",
                StatusCodes.Status401Unauthorized);
        }

        ThrowIfInactiveUser(savedRefreshToken.User!);
        
        var newToken = await CreateTokenCore(savedRefreshToken.User!);

        await refreshTokenRepository.RevokeAsync(savedRefreshToken);
        
        return newToken;
    }

    private async Task<SecurityToken> CreateTokenCore(User user)
    {
        logger.LogDebug("Gerando access token e refresh token para o usuário {UserId}.", user.Id);
        
        var accessToken = tokenManager.GenerateAccessToken(await CreateClaimsAsync(user));
        var refreshTokenValue = tokenManager.GenerateRefreshToken();

        await StoreRefreshTokenAsync(user, refreshTokenValue);

        logger.LogDebug("Tokens gerados com sucesso para o usuário {UserId}.", user.Id);
        
        return new SecurityToken(
            AccessToken: accessToken,
            RefreshToken: refreshTokenValue,
            TokenType: JwtBearerDefaults.AuthenticationScheme,
            ExpiresIn: jwtOptions.Value.LifetimeInSeconds
        );
    }

    private async Task<List<Claim>> CreateClaimsAsync(User user)
    {
        logger.LogDebug("Criando claims para o usuário {UserId}.", user.Id);
        
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow.ToUniversalTime()).ToString(), ClaimValueTypes.Integer64),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        ];

        var roles = await userManager.GetRolesAsync(user);

        logger.LogDebug("Adicionando {RoleCount} roles para o usuário {UserId}.", roles.Count, user.Id);
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    private async Task StoreRefreshTokenAsync(User user, string refreshTokenValue)
    {
        logger.LogDebug("Armazenando refresh token para o usuário {UserId}.", user.Id);
        
        var refreshToken = new RefreshToken
        {
            Token = refreshTokenValue,
            UserId = user.Id
        };

        await refreshTokenRepository.AddAsync(refreshToken);
        
        logger.LogDebug("Refresh token armazenado com sucesso para o usuário {UserId}.", user.Id);
    }

    private static void ThrowIfInactiveUser(User user)
    {
        if (user.Active) return;
        
        throw new ProblemDetailsException(
            "security.authentication-service.user-inactive",
            $"O acesso para o usuário {user.UserName} foi bloqueado porque a conta está marcada como inativa.",
            StatusCodes.Status401Unauthorized
        );
    }
}