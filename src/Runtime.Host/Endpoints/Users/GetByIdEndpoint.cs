using AuthHub.Application.Queries.Users.GetById;
using MediatR;

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
    
    private static async Task<IResult> GetByIdAsync(Guid userId, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetByIdQuery(userId), cancellationToken);
        return Results.Ok(result);
    }
}