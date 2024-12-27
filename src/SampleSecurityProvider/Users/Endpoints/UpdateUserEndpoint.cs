using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SampleSecurityProvider.Abstractions;
using SampleSecurityProvider.ErrorHandling;
using SampleSecurityProvider.Users.Dtos;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Users.Endpoints;

public class UpdateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut("api/users/{userId:Guid}", UpdateUserAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> UpdateUserAsync(
        Guid userId,
        UpdateUserRequest payload, 
        IValidator<UpdateUserRequest> validator,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Results.NotFound(new CustomProblemDetails(
                "users.endpoints.update-user.not-found",
                "Usuário não encontrado.",
                StatusCodes.Status404NotFound
            ));
        }
        
        var validationResult = await validator.ValidateAsync(payload, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new CustomProblemDetails(
                "users.endpoints.update-user.invalid-data",
                "Os dados fornecidos para atualização do cadastro do usuário estão inválidos.", 
                StatusCodes.Status400BadRequest,
                validationResult.Errors.Select(error => new Error(error.ErrorCode, error.ErrorMessage))
            ));
        }
        
        var roleValidationResult = await ValidateRolesExistAsync(payload.Roles, roleManager);
        if (!roleValidationResult.IsSuccess)
        {
            return Results.BadRequest(roleValidationResult.Error);
        }

        var roleAssignmentResult  = await TryAssignRolesAsync(payload.Roles, user, userManager);
        if (!roleAssignmentResult.IsSuccess)
        {
            return Results.BadRequest(roleAssignmentResult.Error);
        }
        
        user.DisplayName = payload.DisplayName;
        user.UserName = payload.Username;
        user.Active = payload.Active;
        
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return Results.BadRequest(new CustomProblemDetails(
                    "users.endpoints.update-user.update-failed",
                    "Falha ao atualizar o cadastro do usuário, verifique os detalhes.",
                    StatusCodes.Status400BadRequest,
                    updateResult.Errors.Select(error => new Error(error.Code, error.Description))
                )
            );
        }
        
        var updateEmailResult = await UpdateEmailIfNeededAsync(payload, user, userManager, cancellationToken);
        
        return !updateEmailResult.IsSuccess
            ? Results.BadRequest(updateEmailResult.Error)
            : Results.NoContent();
    }

    private static async Task<Result> ValidateRolesExistAsync(IEnumerable<string> roles, RoleManager<IdentityRole> roleManager)
    {
        var missingRoles = new List<string>();
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                missingRoles.Add(role);
            }
        }

        if (missingRoles.Count > 0)
        {
            return Result.Failure(new CustomProblemDetails(
                "users.endpoints.update-user.missing-roles",
                $"As roles {string.Join(", ", missingRoles)} não estão cadastradas.",
                StatusCodes.Status400BadRequest)
            );
        }

        return Result.Success();
    }
    
    private static async Task<Result> TryAssignRolesAsync(IEnumerable<string> roles, User user, UserManager<User> userManager)
    {
        var result = await userManager.AddToRolesAsync(user, roles);
        if (!result.Succeeded)
        {
            return Result.Failure(new CustomProblemDetails(
                "users.endpoints.update-user.user-already-in-roles",
                result.Errors.Select(err => err.Description).First(),
                StatusCodes.Status400BadRequest)
            );
        }

        return Result.Success();
    }
    
    private static async Task<Result> UpdateEmailIfNeededAsync(UpdateUserRequest payload, User user, UserManager<User> userManager, CancellationToken cancellationToken)
    {
        if (user.Email == payload.Email) return Result.Success();
        
        var emailAlreadyUsed = await userManager.Users.AnyAsync(x => x.Email == payload.Email, cancellationToken);
        if (emailAlreadyUsed)
        {
            return Result.Failure(new CustomProblemDetails(
                    "users.endpoints.update-user.email-already-used",
                    "O email já está sendo utilizado por outro usuário.",
                    StatusCodes.Status400BadRequest
                )
            );
        }
        
        var token = await userManager.GenerateChangeEmailTokenAsync(user, payload.Email);
        await userManager.ChangeEmailAsync(user, payload.Email, token);

        return Result.Success();
    }
}