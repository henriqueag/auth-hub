using AuthHub.Application.Commands.Users.SendPasswordRecoveryEmail;
using MediatR;

namespace AuthHub.Runtime.Host.Endpoints.Users;

public class SendPasswordRecoveryEmailEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/users/password/forgot", SendPasswordRecoveryEmailAsync)
            .WithTags("Users")
            .WithOpenApi();
    }

    private static async Task<IResult> SendPasswordRecoveryEmailAsync(HttpContext context, SendPasswordRecoveryEmailCommand command, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
        return Results.Accepted();
    }
}