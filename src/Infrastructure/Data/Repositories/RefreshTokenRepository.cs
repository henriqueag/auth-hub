using AuthHub.Domain.Security.Entities;
using AuthHub.Domain.Security.Repositories;
using AuthHub.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AuthHub.Infrastructure.Data.Repositories;

public class RefreshTokenRepository(SqliteDbContext context) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetAsync(string token)
    {
        return context.RefreshTokens
            .Include(x => x.User)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Token == token)
            ;
    }

    public async Task RevokeAsync(RefreshToken refreshToken)
    {
        refreshToken.Revoke();
        await context.SaveChangesAsync();
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }
}