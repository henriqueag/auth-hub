using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SampleSecurityProvider.Abstractions;
using SampleSecurityProvider.ErrorHandling;
using SampleSecurityProvider.Users.Dtos;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Users.Endpoints;

public class ChangePasswordEndpoint : IEndpoint 
{
    public void MapEndpoint(IEndpointRouteBuilder builder) 
    {
        builder.MapPost("api/users/password/change", ChangePasswordAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization();
    }

    private static async Task<IResult> ChangePasswordAsync(
        PasswordRequest payload,
        IValidator<PasswordRequest> validator,
        HttpContext context,
        UserManager<User> userManager,
        CancellationToken cancellationToken)
    {
        var principal = context.User;
        var user = (await userManager.FindByIdAsync(principal.Identity!.Name!))!;

        if (!await userManager.CheckPasswordAsync(user, payload.CurrentPassword!))
        {
            return Results.BadRequest(new CustomProblemDetails(
                "users.endpoints.change-password.invalid-current-password",
                "A senha atual fornecida não corresponde à senha registrada para o usuário.",
                StatusCodes.Status400BadRequest
            ));
        }
        
        var validationResult = await validator.ValidateAsync(payload, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new CustomProblemDetails(
                "users.endpoints.change-password.invalid-data",
                "Os dados fornecidos para a troca de senha do usuário estão inválidos.", 
                StatusCodes.Status400BadRequest,
                validationResult.Errors.Select(error => new Error(error.ErrorCode, error.ErrorMessage))
            ));
        }
        
        var changeResult = await userManager.ChangePasswordAsync(user, payload.CurrentPassword!, payload.Password);
        if (!changeResult.Succeeded)
        {
            return Results.BadRequest(new CustomProblemDetails(
                "users.endpoints.change-password-failed",
                "Não foi possível trocar a senha, verifique os detalhes.",
                StatusCodes.Status400BadRequest,
                changeResult.Errors.Select(err => new Error(err.Code, err.Description))
            ));
        }
        
        return Results.NoContent();
    }
}