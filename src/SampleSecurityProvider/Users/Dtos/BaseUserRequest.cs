namespace SampleSecurityProvider.Users.Dtos;

public abstract record BaseUserRequest
{
    public string DisplayName { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public IEnumerable<string> Roles { get; init; } = [];
}