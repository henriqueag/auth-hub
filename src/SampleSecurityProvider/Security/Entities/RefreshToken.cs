using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Security.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public required string Token { get; init; }
    public required TimeSpan Lifetime { get; init; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; private set; }
    public required string UserId { get; init; }
    public virtual User? User { get; init; }
    
    public bool IsExpired => DateTime.UtcNow > CreatedAt.AddTicks(Lifetime.Ticks);
    public bool IsRevoked => RevokedAt.HasValue;
    
    public void Revoke() => RevokedAt = DateTime.UtcNow;
}