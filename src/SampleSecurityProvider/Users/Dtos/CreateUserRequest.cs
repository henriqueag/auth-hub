namespace SampleSecurityProvider.Users.Dtos;

public record CreateUserRequest : BaseUserRequest
{
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}