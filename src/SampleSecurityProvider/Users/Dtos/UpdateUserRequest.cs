namespace SampleSecurityProvider.Users.Dtos;

public record UpdateUserRequest : BaseUserRequest
{
    public bool Active { get; init; } = false;
}