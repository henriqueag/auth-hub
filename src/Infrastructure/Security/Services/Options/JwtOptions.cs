namespace AuthHub.Domain.Security.Options;

public record JwtOptions
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int LifetimeInSeconds { get; init; }
    public required int RefreshTokenLifetimeInSeconds { get; init; }
}