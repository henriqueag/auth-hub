using AuthHub.Application.Commands.Users.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.Endpoints.Users;

public class CreateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/users", CreateUserAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> CreateUserAsync(HttpContext context, [FromServices] ISender sender, CreateCommand command, CancellationToken cancellationToken)
    {
        command = command with { Link = $"{context.Request.Scheme}://{context.Request.Host}/api/users/password/recovery" };
        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"api/users/{result}", result);
    }
}