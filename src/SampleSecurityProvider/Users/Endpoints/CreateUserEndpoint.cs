using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SampleSecurityProvider.Abstractions;
using SampleSecurityProvider.ErrorHandling;
using SampleSecurityProvider.Users.Dtos;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Users.Endpoints;

public class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/users", CreateUserAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<IResult> CreateUserAsync(
        CreateUserRequest payload, 
        IValidator<CreateUserRequest> validator,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        CancellationToken cancellationToken
        )
    {
        var validationResult = await validator.ValidateAsync(payload, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(new CustomProblemDetails(
                "users.endpoints.create-user.invalid-data",
                "Os dados fornecidos para cadastro do usuário estão inválidos.", 
                StatusCodes.Status400BadRequest,
                validationResult.Errors.Select(error => new Error(error.ErrorCode, error.ErrorMessage))
            ));
        }
        
        var user = new User(payload.DisplayName, payload.Username, payload.Email);
        
        await userManager.CreateAsync(user, payload.Password);
        var assignRolesProblemDetails = await ValidadeAndAssignRolesAsync(user, payload.Roles, roleManager, userManager);
        
        return assignRolesProblemDetails is not null 
            ? TypedResults.BadRequest(assignRolesProblemDetails) 
            : TypedResults.Created($"api/users?userId={user.Id}", user.Id);
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
                "users.endpoints.create-user.missing-roles",
                $"As roles {string.Join(", ", missingRoles)} não estão cadastradas.",
                StatusCodes.Status400BadRequest
            );
        }
        
        await userManager.AddToRolesAsync(user, roles);
        return null;
    }
}