using AuthHub.Domain.Users.Entities;
using AuthHub.Domain.Users.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthHub.Infrastructure.Data.Repositories;

public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync(int page, int pageSize, string? displayName, string? username, string? email, CancellationToken cancellationToken)
    {
        var query = userManager.Users.AsQueryable();
        
        if (!string.IsNullOrEmpty(displayName))
        {
            query = query.Where(u => EF.Functions.Like(u.DisplayName, $"%{displayName}%"));
        }

        if (!string.IsNullOrEmpty(username))
        {
            query = query.Where(u => EF.Functions.Like(u.UserName, $"%{username}%"));
        }

        if (!string.IsNullOrEmpty(email))
        {
            query = query.Where(u => EF.Functions.Like(u.Email, $"%{email}%"));
        }
        
        return await query
            .OrderBy(user => user.DisplayName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return userManager.Users.CountAsync(cancellationToken);
    }
}