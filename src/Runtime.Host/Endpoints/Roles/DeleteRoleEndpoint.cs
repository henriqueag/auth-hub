using AuthHub.Application.Commands.Roles.DeleteRole;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.Endpoints.Roles;

public class DeleteRoleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("api/roles/{id:Guid}", DeleteRoleAsync)
            .WithTags("Roles")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> DeleteRoleAsync([FromServices] ISender sender, Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteRoleCommand(id), cancellationToken);
        return Results.NoContent();
    }
}