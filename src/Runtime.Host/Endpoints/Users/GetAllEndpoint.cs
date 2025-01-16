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

    private static async Task<IResult> GetAllAsync([FromServices] ISender sender, int? page, int? pageSize, string? displayName, string? username, string? email, CancellationToken cancellationToken)
    {
        var query = new GetAllQuery(page, pageSize, displayName, username, email);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }
}