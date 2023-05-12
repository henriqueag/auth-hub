using Microsoft.AspNetCore.Identity;

namespace IdentityWebApi.Models;

public class ApplicationUser : IdentityUser
{
    private ApplicationUser()
    {        
    }

    public ApplicationUser(string username, string email)
    {
        UserName = username;
        Email = email;
    }
}
