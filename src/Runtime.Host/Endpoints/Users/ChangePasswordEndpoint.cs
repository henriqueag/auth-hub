using AuthHub.Application.Commands.Users.ChangePassword;
using MediatR;

namespace AuthHub.Runtime.Host.Endpoints;

public class ChangePasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost("api/users/password/change", ChangePasswordAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization();
    }

    private static async Task<IResult> ChangePasswordAsync(ChangePasswordCommand payload, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(payload, cancellationToken);
        return Results.NoContent();
    }
}