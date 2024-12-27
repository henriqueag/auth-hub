namespace SampleSecurityProvider.Users.Dtos;

public record UserResponse(Guid Id, string DisplayName, string Username, string Email, bool Active, IEnumerable<string> Roles);