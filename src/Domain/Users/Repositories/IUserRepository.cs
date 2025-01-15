using AuthHub.Domain.Users.Entities;

namespace AuthHub.Domain.Users.Repositories;

public interface IUserRepository
{
    IQueryable<User> GetAllAsync(int? page, int? pageSize, string? displayName, string? username, string? email);
    Task<int> CountAsync(CancellationToken cancellationToken);
}