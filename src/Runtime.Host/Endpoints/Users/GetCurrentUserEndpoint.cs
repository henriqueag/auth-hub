using AuthHub.Application.Queries.Users.GetCurrentUser;
using MediatR;

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
    
    private static async Task<IResult> GetCurrentUserAsync(ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(new GetCurrentUserQuery(), cancellationToken));
    }
}