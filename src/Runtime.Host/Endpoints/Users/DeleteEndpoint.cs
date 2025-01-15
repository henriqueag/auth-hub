using AuthHub.Application.Commands.Users.Delete;
using MediatR;

namespace AuthHub.Runtime.Host.Endpoints.Users;

public class DeleteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("api/users/{userId:Guid}", DeleteAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> DeleteAsync(Guid userId, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteCommand(userId), cancellationToken);
        return Results.NoContent();
    }
}