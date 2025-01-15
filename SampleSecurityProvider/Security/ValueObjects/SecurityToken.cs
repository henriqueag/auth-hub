using System.Text.Json.Serialization;

namespace SampleSecurityProvider.Security.ValueObjects;

public record SecurityToken(
    [property: JsonPropertyName("access_token")] string AccessToken, 
    [property: JsonPropertyName("refresh_token")] string RefreshToken, 
    [property: JsonPropertyName("token_type")] string TokenType,
    [property: JsonPropertyName("expires_in")] int ExpiresIn);