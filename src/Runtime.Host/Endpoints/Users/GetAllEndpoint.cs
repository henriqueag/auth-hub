using AuthHub.Application.Queries.Users.GetAll;
using MediatR;

namespace AuthHub.Runtime.Host.Endpoints.Users;

public class GetAllEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/users", GetAllAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization();
    }

    private static async Task<IResult> GetAllAsync(int? page, int? pageSize, string? displayName, string? username, string? email, ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetAllQuery(page, pageSize, displayName, username, email);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }
}