using AuthHub.Application.Commands.Users.ChangePassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.Endpoints.Users;

public class ChangePasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost("api/users/password/change", ChangePasswordAsync)
            .WithTags("Users")
            .WithOpenApi();
    }

    private static async Task<IResult> ChangePasswordAsync([FromServices] ISender sender, ChangePasswordCommand payload, CancellationToken cancellationToken)
    {
        await sender.Send(payload, cancellationToken);
        return Results.NoContent();
    }
}