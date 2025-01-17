using AuthHub.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Commands.Roles.UpdateRole;

public class UpdateRoleCommandHandler(RoleManager<IdentityRole> roleManager) : IRequestHandler<UpdateRoleCommand>
{
    public async Task Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role is null)
        {
            throw new ProblemDetailsException(
                "application.commands.roles.update-role.not-found",
                $"A role com nome {request.Payload.Name} não foi encontrada.", Error.NotFound
            );
        }

        var updateResult = await roleManager.SetRoleNameAsync(role, request.Payload.Name);
        if (!updateResult.Succeeded)
        {
            throw new ProblemDetailsException(
                "application.commands.roles.update-role.update-failure",
                updateResult.Errors.First().Description, Error.BadRequest
            );
        }

        await roleManager.UpdateAsync(role);
    }
}