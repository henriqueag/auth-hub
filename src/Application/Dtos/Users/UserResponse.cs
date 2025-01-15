namespace AuthHub.Application.Dtos.Users;

public record UserResponse(Guid Id, string DisplayName, string Username, string Email, bool Active, IEnumerable<string> Roles, string? Issuer = null);