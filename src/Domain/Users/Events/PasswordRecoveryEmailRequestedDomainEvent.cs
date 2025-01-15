using AuthHub.Domain.Abstractions;
using AuthHub.Domain.Users.Entities;

namespace AuthHub.Domain.Users.Events;

public record PasswordRecoveryEmailRequestedDomainEvent(User User, string Link) : IDomainEvent;