using AuthHub.Application.Commands.Users.Create;
using MediatR;

namespace AuthHub.Runtime.Host.Endpoints.Users;

public class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/users", CreateUserAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> CreateUserAsync(CreateCommand command, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"api/users/{result}", result);
    }
}