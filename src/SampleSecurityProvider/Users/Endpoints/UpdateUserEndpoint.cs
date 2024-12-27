using FluentValidation;
using Microsoft.AspNetCore.Identity;
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
            .RequireAuthorization(policy => policy.RequireRole("Admin"));;
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
            return TypedResults.BadRequest(new CustomProblemDetails(
                "users.endpoints.update-user.invalid-data",
                "Os dados fornecidos para atualização do cadastro do usuário estão inválidos.", 
                StatusCodes.Status400BadRequest,
                validationResult.Errors.Select(error => new Error(error.ErrorCode, error.ErrorMessage))
            ));
        }
        
        var assignRolesProblemDetails = await ValidadeAndAssignRolesAsync(user, payload.Roles, roleManager, userManager);
        if (assignRolesProblemDetails is not null)
        {
            return TypedResults.BadRequest(assignRolesProblemDetails);
        }

        user.DisplayName = payload.DisplayName;
        user.UserName = payload.Username;
        user.Active = payload.Active;
        
        await ChangeEmailAsync(payload, user, userManager);
        await userManager.UpdateAsync(user);
        
        return Results.NoContent();
    }

    private static async Task ChangeEmailAsync(UpdateUserRequest payload, User user, UserManager<User> userManager)
    {
        if (user.Email == payload.Email) return;
        
        var token = await userManager.GenerateChangeEmailTokenAsync(user, payload.Email);
        await userManager.ChangeEmailAsync(user, payload.Email, token);
    }
    
    private static async Task<CustomProblemDetails?> ValidadeAndAssignRolesAsync(
        User user,
        IEnumerable<string> roles,
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager)
    {
        var missingRoles = roles
            .Where(role => !roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
            .ToList();

        if (missingRoles.Count > 0)
        {
            return new CustomProblemDetails(
                "users.endpoints.update-user.missing-roles",
                $"As roles {string.Join(", ", missingRoles)} não estão cadastradas.",
                StatusCodes.Status400BadRequest
            );
        }
        
        await userManager.AddToRolesAsync(user, roles);
        return null;
    }
}