using AuthHub.Domain.Users.Entities;

namespace AuthHub.Domain.Security.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public required string Token { get; init; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; private set; }
    public required string UserId { get; init; }
    public virtual User? User { get; init; }
    
    public bool IsRevoked => RevokedAt.HasValue;
    
    public void Revoke() => RevokedAt = DateTime.UtcNow;
    public bool IsExpired(TimeSpan lifetime) => DateTime.UtcNow > CreatedAt.AddTicks(lifetime.Ticks);
}