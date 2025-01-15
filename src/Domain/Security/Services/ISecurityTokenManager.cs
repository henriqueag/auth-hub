using System.Security.Claims;

namespace AuthHub.Domain.Security.Services;

public interface ISecurityTokenManager
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}