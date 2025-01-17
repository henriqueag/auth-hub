using AuthHub.Application.Queries.Users.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.Endpoints.Users;

public class GetCurrentUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/current-user", GetCurrentUserAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization();
    }
    
    private static async Task<IResult> GetCurrentUserAsync([FromServices] ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new GetCurrentUserQuery(), cancellationToken));
    }
}