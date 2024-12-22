using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Users.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task CraeteAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}