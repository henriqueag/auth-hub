using Microsoft.AspNetCore.Identity;

namespace SampleSecurityProvider.Models;

public sealed class ApplicationUser : IdentityUser
{
    private ApplicationUser() { }

    public ApplicationUser(string username, string email)
    {
        UserName = username;
        Email = email;
    }
}
