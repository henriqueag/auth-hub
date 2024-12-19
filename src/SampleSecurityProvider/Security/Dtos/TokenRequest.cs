namespace SampleSecurityProvider.Security.Dtos;

public record TokenRequest(IDictionary<string, string?> Data)
{
    public string? GrantType => GetValueOrDefault("grant_type");
    public string? Email => GetValueOrDefault("email");
    public string? Password => GetValueOrDefault("password");
    public string? RefreshToken => GetValueOrDefault("refresh_token");
    
    private string? GetValueOrDefault(string key) => Data.TryGetValue(key, out var value) ? value : null;
};