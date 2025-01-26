using AuthHub.Application.Queries.Users.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

    private static async Task<IResult> GetAllAsync([FromServices] ISender sender, int? skip, int? limit, string? q, CancellationToken cancellationToken)
    {
        var query = new GetAllQuery(skip, limit, q);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }
}