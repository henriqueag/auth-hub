namespace SampleSecurityProvider.Users.Dtos;

public record UpdateUserRequest : BaseUserRequest
{
    public bool Active { get; init; } = false;
    public IEnumerable<string> RolesToAdd { get; init; } = [];
    public IEnumerable<string> RolesToRemove { get; init; } = [];
}