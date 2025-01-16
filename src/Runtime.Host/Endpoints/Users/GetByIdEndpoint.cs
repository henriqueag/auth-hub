using AuthHub.Application.Queries.Users.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.Endpoints.Users;

public class GetByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/users/{userId:Guid}", GetByIdAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization();
    }
    
    private static async Task<IResult> GetByIdAsync([FromServices] ISender sender, Guid userId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetByIdQuery(userId), cancellationToken);
        return Results.Ok(result);
    }
}