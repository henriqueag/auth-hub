namespace SampleSecurityProvider.Users.Dtos;

public record PasswordRequest
{
    public string? CurrentPassword { get; init; }
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}