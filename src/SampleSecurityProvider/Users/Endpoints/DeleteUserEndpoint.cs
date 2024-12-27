using Microsoft.AspNetCore.Identity;
using SampleSecurityProvider.Abstractions;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Users.Endpoints;

public class DeleteUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("api/users/{userId:Guid}", DeleteUserAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> DeleteUserAsync(Guid userId, UserManager<User> userManager)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Results.NoContent();
        }

        await userManager.DeleteAsync(user);
        
        return Results.NoContent();
    }
}