using AuthHub.Domain.Abstractions;
using AuthHub.Domain.Users.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Commands.Roles.DeleteRole;

public class DeleteRoleCommandHandler(RoleManager<IdentityRole> roleManager, UserManager<User> userManager) : IRequestHandler<DeleteRoleCommand>
{
    public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role is null)
        {
            return;
        }

        var usersInRole = await userManager.GetUsersInRoleAsync(role.Name ?? "");
        if (usersInRole.Count > 0)
        {
            throw new ProblemDetailsException(
                "application.commands.roles.delete-role.users-in-role",
                "Para excluir esta role, é necessário remover todas as associações com usuários. Em seguida, tente excluir a role novamente.", 
                Error.BadRequest
            );
        }

        await roleManager.DeleteAsync(role);
    }
}