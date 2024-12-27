using SampleSecurityProvider.Security.Entities;

namespace SampleSecurityProvider.Security.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetAsync(string token);
    Task RevokeAsync(RefreshToken refreshToken);
    Task AddAsync(RefreshToken refreshToken);
}