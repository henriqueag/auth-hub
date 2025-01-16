using AuthHub.Application.Commands.Users.SendPasswordRecoveryEmail;
using AuthHub.Domain.Abstractions;
using AuthHub.Domain.Users.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Commands.Users.Create;

public class CreateCommandHandler(
    IValidator<CreateCommand> validator,
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    ISender sender) 
    : IRequestHandler<CreateCommand, string>
{
    public async Task<string> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ProblemDetailsException(
                "application.commands.users.create.invalid-data",
                "Os dados fornecidos para cadastro do usuário estão inválidos.",
                Error.BadRequest, validationResult.Errors.Select(error => new Error(error.ErrorCode, error.ErrorMessage)));
        }
        
        await ValidateRolesExistAsync(request.Roles, roleManager);
        
        var emailAlreadyUsed = userManager.Users.Any(x => x.Email == request.Email);
        if (emailAlreadyUsed)
        {
            throw new ProblemDetailsException(
                "application.commands.users.create-user.email-already-used",
                "O email já está sendo utilizado por outro usuário.",
                Error.BadRequest);
        }
        
        var user = new User(request.DisplayName, request.Username, request.Email);
        
        var creationResult = await userManager.CreateAsync(user);
        if (!creationResult.Succeeded)
        {
            throw new ProblemDetailsException(
                "application.commands.users.create-user.creation-failed",
                "Falha ao cadastrar o usuário, verifique os detalhes.",
                Error.BadRequest, creationResult.Errors.Select(err => new Error(err.Code, err.Description)));
        }
        
        await userManager.AddToRolesAsync(user, request.Roles);
        await sender.Send(new SendPasswordRecoveryEmailCommand(user.Email, request.Link), cancellationToken);
        
        return user.Id;
    }
    
    private static async Task ValidateRolesExistAsync(IEnumerable<string> roles, RoleManager<IdentityRole> roleManager)
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
            throw new ProblemDetailsException(
                "application.commands.users.create-user.missing-roles",
                $"As roles {string.Join(", ", missingRoles)} não estão cadastradas.",
                Error.BadRequest);
        }
    }
}