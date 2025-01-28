using AuthHub.Domain.Users.Entities;
using AuthHub.Domain.Users.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthHub.Infrastructure.Data.Repositories;

public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync(int skip, int limit, string? query, CancellationToken cancellationToken)
    {
        var queryable = userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(query))
        {
            queryable = queryable.Where(user =>
                ToLower(user.DisplayName).Contains(ToLower(query)) ||
                ToLower(user.UserName!).Contains(ToLower(query)) ||
                ToLower(user.Email!).Contains(ToLower(query))
            );
        }

        queryable = queryable.OrderBy(user => user.DisplayName);

        if (skip > 0)
        {
            queryable = queryable.Skip(skip);
        }

        return await queryable.Take(limit).ToListAsync(cancellationToken);        
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return userManager.Users.CountAsync(cancellationToken);
    }

    private static string ToLower(string input) => input.ToLower();
}