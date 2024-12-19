using System.Text.Json;

namespace SampleSecurityProvider.Security.ValueObjects;

public record RefreshToken(string Value, TimeSpan Lifetime)
{
    public string Serialize() => JsonSerializer.Serialize(this);
    
    public static RefreshToken Deserialize(string json) => JsonSerializer.Deserialize<RefreshToken>(json)!;
}