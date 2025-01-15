using AuthHub.Domain.Security.Entities;
using AuthHub.Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthHub.Infrastructure.Data.Contexts;

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

        builder.Entity<User>(userBuilder =>
        {
            userBuilder.ToTable("Users");

            userBuilder.Property(x => x.DisplayName)
                .HasMaxLength(128)
                .IsRequired();
        });
        
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

        builder.Ignore(x => x.IsRevoked);
        
        builder.Property(x => x.Token)
            .HasMaxLength(1024)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}