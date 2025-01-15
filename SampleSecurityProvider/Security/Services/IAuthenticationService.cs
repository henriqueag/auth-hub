using SampleSecurityProvider.Security.ValueObjects;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Security.Services;

public interface IAuthenticationService
{
    Task<SecurityToken> CreateTokenAsync(User user, string password);
    Task<SecurityToken> RefreshTokenAsync(string refreshToken);
}