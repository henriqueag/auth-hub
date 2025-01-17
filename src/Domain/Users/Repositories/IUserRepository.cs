using AuthHub.Domain.Users.Entities;

namespace AuthHub.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync(int page, int pageSize, string? displayName, string? username, string? email, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
}