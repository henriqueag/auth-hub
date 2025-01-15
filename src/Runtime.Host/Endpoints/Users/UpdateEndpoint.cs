using AuthHub.Application.Commands.Users.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.Endpoints.Users;

public class UpdateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut("api/users/{userId:Guid}", UpdateUserAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> UpdateUserAsync(Guid userId, UpdateCommand payload, [FromServices] ISender sender, CancellationToken cancellationToken)
    {
        payload = payload with { UserId = userId };
        await sender.Send(payload, cancellationToken);
        return Results.NoContent();
    }
}