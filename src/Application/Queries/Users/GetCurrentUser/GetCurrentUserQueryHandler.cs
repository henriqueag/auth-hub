using System.IdentityModel.Tokens.Jwt;
using AuthHub.Application.Dtos.Users;
using AuthHub.Domain.Users.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Queries.Users.GetCurrentUser;

public class GetCurrentUserQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager) : IRequestHandler<GetCurrentUserQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var principal = httpContextAccessor.HttpContext!.User;
        var user = await userManager.FindByIdAsync(principal.Identity!.Name!);
        var roles = await userManager.GetRolesAsync(user!);
        var issuer = principal.FindFirst(JwtRegisteredClaimNames.Iss)?.Value;

        return new UserResponse(Guid.Parse(user!.Id), user.DisplayName, user.UserName!, user.Email!, user.Active, roles, issuer);
    }
}