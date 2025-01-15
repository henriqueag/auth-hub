using AuthHub.Application.Dtos;
using AuthHub.Application.Dtos.Users;
using AuthHub.Domain.Users.Entities;
using AuthHub.Domain.Users.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Queries.Users.GetAll;

public class GetAllQueryHandler(IUserRepository userRepository, UserManager<User> userManager) : IRequestHandler<GetAllQuery, PaginatedResponse<UserResponse>>
{
    public async Task<PaginatedResponse<UserResponse>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 10;
        
        var query = userRepository.GetAllAsync(page, pageSize, request.DisplayName, request.Username, request.Email);
        
        var totalItems = await userRepository.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var paginatedData = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
            ;
        
        var response = new List<UserResponse>();
        
        foreach (var user in paginatedData)
        {
            var userResponse = new UserResponse(Guid.Parse(user.Id), user.DisplayName, user.UserName!, user.Email!, user.Active, await userManager.GetRolesAsync(user));
            response.Add(userResponse);
        }

        return new PaginatedResponse<UserResponse>
        {
            Items = response,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }
}