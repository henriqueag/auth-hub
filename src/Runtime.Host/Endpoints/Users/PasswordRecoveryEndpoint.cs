using AuthHub.Application.Commands.Users.PasswordRecovery;
using MediatR;

namespace AuthHub.Runtime.Host.Endpoints.Users;

public class PasswordRecoveryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/users/password/recovery", PasswordRecoveryAsync)
            .WithTags("Users")
            .WithOpenApi();
    }

    private static async Task<IResult> PasswordRecoveryAsync(string email, string token, ISender sender, PasswordRecoveryCommand command, CancellationToken cancellationToken)
    {
        await sender.Send(new PasswordRecoveryCommand(email, token), cancellationToken);
        return Results.Ok();
    }
}