using MassTransit;
using Microsoft.AspNetCore.Identity;
using SampleSecurityProvider.Abstractions;
using SampleSecurityProvider.ErrorHandling;
using SampleSecurityProvider.Users.Dtos;
using SampleSecurityProvider.Users.Entities;
using SampleSecurityProvider.Users.Events;

namespace SampleSecurityProvider.Users.Endpoints;

public class SendPasswordRecoveryEmailEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/users/password/forgot", SendPasswordRecoveryEmailAsync)
            .WithTags("Users")
            .WithOpenApi();
    }

    private static async Task<IResult> SendPasswordRecoveryEmailAsync(
        HttpContext context,
        PasswordRecoveryRequest payload,
        UserManager<User> userManager,
        IBus bus)
    {
        var user = await userManager.FindByEmailAsync(payload.Email ?? "");
        if (user is null)
        {
            return Results.NotFound(new CustomProblemDetails(
                "user.endpoints.password-recovery.user-not-found",
                "Não foi possível localizar um usuário com o e-mail fornecido.",
                StatusCodes.Status404NotFound
            ));
        }

        if (!user.Active)
        {
            return Results.BadRequest(new CustomProblemDetails(
                "user.endpoints.password-recovery.user-inactive",
                "O usuário associado ao e-mail fornecido está inativo e não pode realizar a recuperação de senha.",
                StatusCodes.Status400BadRequest
            ));
        }
        
        var recoveryToken = await userManager.GeneratePasswordResetTokenAsync(user);

        var request = context.Request;
        var link = $"{request.Scheme}://{request.Host}/api/users/password/recovery?email={user.Email}&token={recoveryToken}";

        await bus.Publish(new PasswordRecoveryEmailRequested(user, link) as object);
        
        return Results.Accepted();
    }
}