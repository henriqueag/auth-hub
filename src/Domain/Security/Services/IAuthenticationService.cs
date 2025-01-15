using AuthHub.Domain.Users.Entities;
using SecurityToken = SecurityToken;

namespace SampleSecurityProvider.Security.Services;

public interface IAuthenticationService
{
    Task<SecurityToken> CreateTokenAsync(User user, string password);
    Task<SecurityToken> RefreshTokenAsync(string refreshToken);
}