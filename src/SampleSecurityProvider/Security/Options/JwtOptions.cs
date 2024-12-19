namespace SampleSecurityProvider.Security.Options;

public record JwtOptions
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string PrivateKeyPath { get; init; }
    public required string PublicKeyPath { get; init; }
    public required int LifetimeInSeconds { get; init; }
    public required int RefreshTokenLifetimeInSeconds { get; init; }
}