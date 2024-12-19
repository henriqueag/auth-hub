using System.Security.Claims;

namespace SampleSecurityProvider.Security.Services;

public interface ISecurityTokenManager
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}