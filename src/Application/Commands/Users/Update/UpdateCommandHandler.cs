using AuthHub.Domain.Abstractions;
using AuthHub.Domain.Users.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Commands.Users.Update;

public class UpdateCommandHandler(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IValidator<UpdateCommand> validator)
    : IRequestHandler<UpdateCommand>
{
    public async Task Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            throw new ProblemDetailsException(
                "application.commands.users.update-user.not-found",
                "Usuário não encontrado.",
                Error.NotFound);
        }
        
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ProblemDetailsException(
                "application.commands.users.update-user.invalid-data",
                "Os dados fornecidos para atualização do cadastro do usuário estão inválidos.",
                Error.BadRequest, validationResult.Errors.Select(error => new Error(error.ErrorCode, error.ErrorMessage)));
        }

        request.RolesToAdd ??= [];
        request.RolesToRemove ??= [];
        
        await ValidateRolesExistAsync(request.RolesToAdd);

        await AddRolesAsync(request.RolesToAdd, user);
        await RemoveRolesAsync(request.RolesToRemove, user);
        
        user.DisplayName = request.DisplayName;
        user.UserName = request.Username;
        user.Active = request.Active;
        
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            throw new ProblemDetailsException(
                "application.commands.users.update-user.update-failed",
                "Falha ao atualizar o cadastro do usuário, verifique os detalhes.",
                Error.BadRequest, updateResult.Errors.Select(error => new Error(error.Code, error.Description)));
        }
        
        await UpdateEmailIfNeededAsync(request, user, cancellationToken);
    }
    
    private async Task ValidateRolesExistAsync(IEnumerable<string> roles)
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
                "application.commands.users.update-user.missing-roles",
                $"As roles {string.Join(", ", missingRoles.Select(role => $"'{role}'"))} não estão cadastradas.",
                Error.BadRequest);
        }
    }
    
    private async Task AddRolesAsync(IEnumerable<string> roles, User user)
    {
        foreach (var role in roles)
        {
            var isInRole = await userManager.IsInRoleAsync(user, role);
            if (!isInRole)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }

    private async Task RemoveRolesAsync(IEnumerable<string> roles, User user)
    {
        foreach (var role in roles)
        {
            var isInRole = await userManager.IsInRoleAsync(user, role);
            if (isInRole)
            {
                await userManager.RemoveFromRoleAsync(user, role);
            }
        }
    }
    
    private async Task UpdateEmailIfNeededAsync(UpdateCommand payload, User user, CancellationToken cancellationToken)
    {
        if (user.Email == payload.Email) return;
        
        var emailAlreadyUsed = userManager.Users.Any(x => x.Email == payload.Email);
        if (emailAlreadyUsed)
        {
            throw new ProblemDetailsException(
                "application.commands.users.update-user.email-already-used",
                "O email já está sendo utilizado por outro usuário.",
                Error.BadRequest);
        }
        
        var token = await userManager.GenerateChangeEmailTokenAsync(user, payload.Email);
        await userManager.ChangeEmailAsync(user, payload.Email, token);
    }
}