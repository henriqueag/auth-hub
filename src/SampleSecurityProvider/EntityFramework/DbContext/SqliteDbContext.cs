using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleSecurityProvider.Security.Entities;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.EntityFramework.DbContext;

public sealed class SqliteDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    public required DbSet<RefreshToken> RefreshTokens { get; init; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureIdentity(builder);
        ConfigureRefreshToken(builder.Entity<RefreshToken>());
    }

    private static void ConfigureIdentity(ModelBuilder builder)
    {
        builder.Ignore<IdentityRoleClaim<string>>();
        builder.Ignore<IdentityUserLogin<string>>();
        builder.Ignore<IdentityUserToken<string>>();
        
        builder.Entity<User>()
            .ToTable("Users");
        
        builder.Entity<IdentityRole>()
            .ToTable("Roles");
        
        builder.Entity<IdentityUserClaim<string>>()
            .ToTable("UserClaims");

        builder.Entity<IdentityUserRole<string>>()
            .ToTable("UserRoles");
    }


    private static void ConfigureRefreshToken(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("UserRefreshTokens");
        
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Token);
        
        builder.Property(x => x.Token)
            .HasMaxLength(1024)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}