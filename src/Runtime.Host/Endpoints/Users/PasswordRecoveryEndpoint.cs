using AuthHub.Application.Commands.Users.PasswordRecovery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.Endpoints.Users;

public class PasswordRecoveryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/users/password/recovery", PasswordRecoveryAsync)
            .WithTags("Users")
            .WithOpenApi();
    }

    private static async Task<IResult> PasswordRecoveryAsync([FromServices] ISender sender, string email, string token, PasswordRecoveryCommand payload, CancellationToken cancellationToken)
    {
        payload = payload with { Email = email, Token = token };
        await sender.Send(payload, cancellationToken);
        return Results.Ok();
    }
}