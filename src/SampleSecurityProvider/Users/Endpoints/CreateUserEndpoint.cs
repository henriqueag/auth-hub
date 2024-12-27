using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            return Results.BadRequest(new CustomProblemDetails(
                "users.endpoints.create-user.invalid-data",
                "Os dados fornecidos para cadastro do usuário estão inválidos.", 
                StatusCodes.Status400BadRequest,
                validationResult.Errors.Select(error => new Error(error.ErrorCode, error.ErrorMessage))
            ));
        }

        var roleValidationResult = await ValidateRolesExistAsync(payload.Roles, roleManager);
        if (!roleValidationResult.IsSuccess)
        {
            return Results.BadRequest(roleValidationResult.Error);
        }
        
        var emailAlreadyUsed = await userManager.Users.AnyAsync(x => x.Email == payload.Email, cancellationToken);
        if (emailAlreadyUsed)
        {
            return Results.BadRequest(new CustomProblemDetails(
                    "users.endpoints.create-user.email-already-used",
                    "O email já está sendo utilizado por outro usuário.",
                    StatusCodes.Status400BadRequest
                )
            );
        }
        
        var user = new User(payload.DisplayName, payload.Username, payload.Email);
        
        var creationResult = await userManager.CreateAsync(user, payload.Password);
        if (!creationResult.Succeeded)
        {
            return Results.BadRequest(new CustomProblemDetails(
                    "users.endpoints.create-user.creation-failed",
                    "Falha ao cadastrar o usuário, verifique os detalhes.",
                    StatusCodes.Status400BadRequest,
                    creationResult.Errors.Select(err => new Error(err.Code, err.Description))
                )
            );
        }
        
        await userManager.AddToRolesAsync(user, payload.Roles);
        
        return Results.Created($"api/users/{user.Id}", user.Id);
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
                "users.endpoints.create-user.missing-roles",
                $"As roles {string.Join(", ", missingRoles)} não estão cadastradas.",
                StatusCodes.Status400BadRequest)
            );
        }

        return Result.Success();
    }
}