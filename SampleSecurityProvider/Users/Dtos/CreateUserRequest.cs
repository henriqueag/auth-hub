namespace SampleSecurityProvider.Users.Dtos;

public record CreateUserRequest : BaseUserRequest
{
    public IEnumerable<string> Roles { get; init; } = [];
}