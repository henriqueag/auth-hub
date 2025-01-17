using AuthHub.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Queries.Roles.GetRoleById;

public class GetRoleByIdQueryHandler(RoleManager<IdentityRole> roleManager) : IRequestHandler<GetRoleByIdQuery, object>
{
    public async Task<object> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role is null)
        {
            throw new ProblemDetailsException(
                "application.queries.roles.get-role-by-name.not-found",
                "Role não foi encontrada.", Error.NotFound
            );
        }

        return new { role.Id, role.Name };
    }
}