using AuthHub.Domain.Security.Entities;

namespace AuthHub.Domain.Security.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetAsync(string token);
    Task RevokeAsync(RefreshToken refreshToken);
    Task AddAsync(RefreshToken refreshToken);
}