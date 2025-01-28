using AuthHub.Domain.Users.Entities;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthHub.Infrastructure.Data.Contexts;

public class DatabaseInitializer(IServiceScopeFactory factory)
{
    public async Task InitializeAsync()
    {
        var scope = factory.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<SqliteDbContext>();

        await context.Database.MigrateAsync();
        await SeedAsync(services);
    }

    private static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<User>>();

        if (await userManager.Users.CountAsync() > 40) return;

        // var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // User[] users =
        // [
        //     new("Administrador", "admin", "admin@email.com"),
        //     new("Usuário de Teste 1", "test1", "test1@email.com")
        // ];

        // IdentityRole[] roles =
        // [
        //     new("Admin"),
        //     new("Default")
        // ];

        // foreach (var user in users) await userManager.CreateAsync(user, "test@123");
        // foreach (var role in roles) await roleManager.CreateAsync(role);

        // await userManager.AddToRoleAsync(users[0], roles[0].Name!);
        // await userManager.AddToRoleAsync(users[1], roles[1].Name!);

        foreach (var user in GetFakeUsers())
        {
            await userManager.CreateAsync(user, "test@123");
        }
    }

    private static List<User> GetFakeUsers() 
    {
        return new Faker<User>()
            .CustomInstantiator((faker) => new User(faker.Person.FullName, faker.Person.UserName, faker.Person.Email) 
            {
                Active = faker.Random.Int() % 2 == 0,
                EmailConfirmed = true
            })
            .Generate(40);
    }
}