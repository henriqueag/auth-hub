using SampleSecurityProvider.Models;
using SampleSecurityProvider.Security.ValueObjects;

namespace SampleSecurityProvider.Security.Services;

public interface IAuthenticationService
{
    Task<SecurityToken> CreateTokenAsync(ApplicationUser user, string password);
    Task<SecurityToken> RefreshTokenAsync(ApplicationUser user, string refreshToken);
}