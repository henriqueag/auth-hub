using AuthHub.Application.Dtos.Users;
using AuthHub.Domain.Abstractions;
using AuthHub.Domain.Users.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Queries.Users.GetById;

public class GetByIdQueryHandler(UserManager<User> userManager) : IRequestHandler<GetByIdQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            throw new ProblemDetailsException(
                "application.queries.users.get-by-id.user-not-found",
                $"O usuário ID {request.UserId} não foi encontrado.", Error.NotFound);
        } 
        
        return new UserResponse(Guid.Parse(user.Id), user.DisplayName, user.UserName!, user.Email!, user.Active, await userManager.GetRolesAsync(user));
    }
}