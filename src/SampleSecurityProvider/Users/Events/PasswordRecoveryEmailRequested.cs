using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Users.Events;

public record PasswordRecoveryEmailRequested(User User, string Link);