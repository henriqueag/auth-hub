using System.Linq.Expressions;
using AuthHub.Application.Dtos;
using AuthHub.Application.Dtos.Users;
using AuthHub.Domain.Users.Entities;
using AuthHub.Domain.Users.Repositories;
using Bogus;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Queries.Users.GetAll;

public class GetAllQueryHandler(IUserRepository userRepository, UserManager<User> userManager) : IRequestHandler<GetAllQuery, PagedResponse<UserResponse>>
{    
    public async Task<PagedResponse<UserResponse>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var skip = request.Skip ?? 0;
        var limit = request.Limit ?? 10;
        
        var totalRecords = await userRepository.CountAsync(cancellationToken);        
        var paginatedData = await userRepository.GetAllAsync(skip, limit, request.Query, cancellationToken);
        
        var response = new List<UserResponse>();
        
        foreach (var user in paginatedData)
        {
            var userResponse = new UserResponse(Guid.Parse(user.Id), user.DisplayName, user.UserName!, user.Email!, user.Active, await userManager.GetRolesAsync(user));
            response.Add(userResponse);
        }
        
        return new PagedResponse<UserResponse>
        {
            Items = response,
            TotalRecords = totalRecords
        };
    }    
}