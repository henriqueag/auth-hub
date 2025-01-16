using AuthHub.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Commands.Roles.CreateRole;

public class CreateRoleCommandHandler(RoleManager<IdentityRole> roleManager) : IRequestHandler<CreateRoleCommand, string>
{
    public async Task<string> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var creationResult = await roleManager.CreateAsync(new IdentityRole(request.Payload.Name!));
        if (!creationResult.Succeeded)
        {
            throw new ProblemDetailsException(
                "application.commands.roles.create-role.creation-failure",
                creationResult.Errors.First().Description, Error.BadRequest
            );
        }

        return request.Payload.Name!;
    }
}