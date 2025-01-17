namespace AuthHub.Application.Dtos.Users;

public record CreateUserRequest : BaseUserRequest
{
    public IEnumerable<string> Roles { get; init; } = [];
}