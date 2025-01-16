using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace AuthHub.Application.Dtos.Security;

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