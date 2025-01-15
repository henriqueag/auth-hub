using AuthHub.Domain.Users.Entities;
using AuthHub.Domain.Users.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthHub.Infrastructure.Data.Repositories;

public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    public Task<IEnumerable<User>> GetAllAsync(int? page, int? pageSize, string? displayName, string? username, string? email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return userManager.Users.CountAsync(cancellationToken);
    }
}