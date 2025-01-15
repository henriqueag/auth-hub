using AuthHub.Application.Commands.Users.SendPasswordRecoveryEmail;
using MassTransit;
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

    private static async Task<IResult> SendPasswordRecoveryEmailAsync(HttpContext context, ISender sender, SendPasswordRecoveryEmailCommand command,  CancellationToken cancellationToken)
    {
        command = command with { Link = $"{context.Request.Scheme}://{context.Request.Host}/api/users/password/recovery" };
        await sender.Send(command, cancellationToken);
        return Results.Accepted();
    }
}