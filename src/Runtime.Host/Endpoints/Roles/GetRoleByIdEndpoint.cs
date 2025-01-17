using AuthHub.Application.Queries.Roles.GetRoleById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.Endpoints.Roles;

public class GetRoleByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/roles/{id:Guid}", GetByIdAsync)
            .WithTags("Roles")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> GetByIdAsync([FromServices] ISender sender, Guid id, CancellationToken cancellationToken)
    {
        var role = await sender.Send(new GetRoleByIdQuery(id), cancellationToken);
        return Results.Ok(role);
    }
}