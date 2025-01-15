using Microsoft.EntityFrameworkCore;
using SampleSecurityProvider.EntityFramework.DbContext;
using SampleSecurityProvider.Security.Entities;
using SampleSecurityProvider.Security.Repositories;

namespace SampleSecurityProvider.EntityFramework.Repositories;

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