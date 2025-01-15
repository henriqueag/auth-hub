using AuthHub.Domain.Abstractions;

namespace AuthHub.Domain.Users;

public record PasswordRecoveryEmailRequestedDomainEvent(User User, string Link) : IDomainEvent;