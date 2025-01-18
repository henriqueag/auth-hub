namespace AuthHub.Infrastructure.Security.Options;

public record JwtOptions
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required double LifetimeInMinutes { get; init; }
    public required double RefreshTokenLifetimeInMinutes { get; init; }
}