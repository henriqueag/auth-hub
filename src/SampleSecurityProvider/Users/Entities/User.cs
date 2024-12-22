using Microsoft.AspNetCore.Identity;

namespace SampleSecurityProvider.Users.Entities;

public sealed class User : IdentityUser
{
    public User(string username, string email)
    {
        UserName = username;
        Email = email;
    }
}
