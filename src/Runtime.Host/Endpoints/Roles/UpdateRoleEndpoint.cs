using AuthHub.Application.Commands.Roles.UpdateRole;
using AuthHub.Application.Dtos.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.Endpoints.Roles;

public class UpdateRoleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut("api/roles/{id:Guid}", UpdateRoleAsync)
            .WithTags("Roles")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> UpdateRoleAsync([FromServices] ISender sender, Guid id, RoleRequest payload, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateRoleCommand(id, payload), cancellationToken); 
        return Results.NoContent();
    }
}