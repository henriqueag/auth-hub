using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using SampleSecurityProvider.Abstractions;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Users.Endpoints;

public class CurrentUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/current-user", GetCurrentUserAsync)
            .WithTags("User")
            .WithOpenApi()
            .RequireAuthorization();
    }

    private static async Task<IResult> GetCurrentUserAsync(HttpContext context, UserManager<User> userManager)
    {
        var principal = context.User;
        var user = await userManager.FindByIdAsync(principal.Identity!.Name!);
        var roles = await userManager.GetRolesAsync(user!);
        var issuer = principal.FindFirst(JwtRegisteredClaimNames.Iss)?.Value;
        
        var response = new { user!.Id, user.DisplayName, user.UserName, user.Email, roles, issuer };
        return Results.Ok(response);
    }
}