using Microsoft.Extensions.Primitives;

namespace SampleSecurityProvider.Security.Dtos;

public record TokenRequest(IFormCollection Form)
{
    public string? GrantType => GetValueOrDefault("grant_type");
    public string? Email => GetValueOrDefault("email");
    public string? Password => GetValueOrDefault("password");
    public string? RefreshToken => GetValueOrDefault("refresh_token");

    private string? GetValueOrDefault(string key)
    {
        var value = Form[key];
        return value != StringValues.Empty ? value.ToString() : null;
    }
};