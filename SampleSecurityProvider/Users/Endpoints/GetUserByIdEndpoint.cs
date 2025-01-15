using Microsoft.AspNetCore.Identity;
using SampleSecurityProvider.Abstractions;
using SampleSecurityProvider.Users.Dtos;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Users.Endpoints;

public class GetUserByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/users/{userId:Guid}", GetUserAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization();
    }

    private static async Task<IResult> GetUserAsync(Guid userId, UserManager<User> userManager)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        return user is null 
            ? Results.NotFound()
            : Results.Ok(await MapToResponseAsync(user, userManager));
    }

    private static async Task<UserResponse> MapToResponseAsync(User user, UserManager<User> userManager)
    {
        return new UserResponse(Guid.Parse(user.Id), user.DisplayName, user.UserName!, user.Email!, user.Active, await userManager.GetRolesAsync(user));
    }
}