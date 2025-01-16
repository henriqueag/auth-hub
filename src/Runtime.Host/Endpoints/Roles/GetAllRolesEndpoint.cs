using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthHub.Runtime.Host.Endpoints.Roles;

public class GetAllRolesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/roles", GetAllAsync)
            .WithTags("Roles")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> GetAllAsync(RoleManager<IdentityRole> roleManager, CancellationToken cancellationToken)
    {
        return Results.Ok(
            await roleManager.Roles
                .Select(role => new { role.Id, role.Name })
                .ToListAsync(cancellationToken)
        );
    }
}