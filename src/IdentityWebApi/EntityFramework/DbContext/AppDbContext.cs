using IdentityWebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityWebApi.EntityFramework;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRoleClaim<string>>()
            .ToTable("RoleClaims");

        builder.Entity<IdentityUserClaim<string>>()
            .ToTable("UserClaims");

        builder.Entity<IdentityUserLogin<string>>()
            .ToTable("UserLogins");

        builder.Entity<IdentityUserRole<string>>()
            .ToTable("UserRoles");

        builder.Entity<IdentityUserToken<string>>()
            .ToTable("UserTokens");

        builder.Entity<IdentityRole>()
            .ToTable("Roles");

        builder.Entity<ApplicationUser>()
            .ToTable("Users");
    }
}