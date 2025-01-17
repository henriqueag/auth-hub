using AuthHub.Domain.Security.ValueObjects;
using AuthHub.Domain.Users.Entities;

namespace AuthHub.Domain.Security.Services;

public interface IAuthenticationService
{
    Task<SecurityToken> CreateTokenAsync(User user, string password);
    Task<SecurityToken> RefreshTokenAsync(string refreshToken);
}