using AuthHub.Domain.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Domain.Users;

public sealed class User : IdentityUser
{
    private readonly List<IDomainEvent> _domainEvents = [];
   
    // Ef Core
    public User() { }
    
    public User(string displayName, string username, string email)
    {
        DisplayName = displayName;
        UserName = username;
        Email = email;
        EmailConfirmed = true;
        Active = true;
    }

    public string DisplayName { get; set; } = null!;
    public bool Active { get; set; }
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;
    
    public void AddDomainEvent(IDomainEvent domainEvent) 
        => _domainEvents.Add(domainEvent);
    
    public void ClearDomainEvents()
        => _domainEvents.Clear();
}