using AuthHub.Domain.Users.Entities;

namespace AuthHub.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync(int skip, int limit, string? query, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
}