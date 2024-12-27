using Microsoft.AspNetCore.Identity;

namespace SampleSecurityProvider.Users.Entities;

public sealed class User : IdentityUser
{
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
}
