using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SampleSecurityProvider.Abstractions;
using SampleSecurityProvider.ErrorHandling;
using SampleSecurityProvider.Users.Dtos;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Users.Endpoints;

public class PasswordRecoveryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/users/password/recovery", PasswordRecoveryAsync)
            .WithTags("Users")
            .WithOpenApi();
    }

    private static async Task<IResult> PasswordRecoveryAsync(string email, string token, PasswordRequest payload, UserManager<User> userManager, IValidator<PasswordRequest> validator, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Results.NotFound(new CustomProblemDetails(
                "users.endpoints.password-recovery.not-found",
                $"Nenhum usuário não encontrado com o email {email}.",
                StatusCodes.Status404NotFound
            ));
        } 
        
        var validationResult = await validator.ValidateAsync(payload, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new CustomProblemDetails(
                "users.endpoints.password-recovery.invalid-data",
                "Os dados fornecidos para a recuperação de senha do usuário estão inválidos.", 
                StatusCodes.Status400BadRequest,
                validationResult.Errors.Select(error => new Error(error.ErrorCode, error.ErrorMessage))
            ));
        }
        
        var resetResult = await userManager.ResetPasswordAsync(user, token, payload.Password);
        if (!resetResult.Succeeded)
        {
            return Results.BadRequest(new CustomProblemDetails(
                "users.endpoints.password-recovery-failed",
                resetResult.Errors.Select(err => err.Description).First(),
                StatusCodes.Status400BadRequest
            ));
        }
        
        return Results.Ok();
    }
}