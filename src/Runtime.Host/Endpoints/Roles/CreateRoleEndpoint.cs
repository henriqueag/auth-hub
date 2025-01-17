using AuthHub.Application.Commands.Roles.CreateRole;
using AuthHub.Application.Dtos.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.Endpoints.Roles;

public class CreateRoleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/roles", CreateRoleAsync)
            .WithTags("Roles")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> CreateRoleAsync([FromServices] ISender sender, RoleRequest payload, CancellationToken cancellationToken)
    {
        var command = new CreateRoleCommand(payload);
        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"api/roles/{result}", result);
    }
}