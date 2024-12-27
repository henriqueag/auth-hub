using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SampleSecurityProvider.Abstractions;
using SampleSecurityProvider.Users.Dtos;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Users.Endpoints;

public class GetAllUsersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/users", GetUsersAsync)
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization();
    }

    private static async Task<IResult> GetUsersAsync(UserManager<User> userManager, int? page, int? pageSize, string? displayName, string? username, string? email)
    {
        page ??= 1;
        pageSize ??= 10;
        
        var query = userManager.Users.AsQueryable();
        
        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize.Value);

        if (!string.IsNullOrEmpty(displayName))
        {
            query = query.Where(u => EF.Functions.Like(u.DisplayName, $"%{displayName}%"));
        }

        if (!string.IsNullOrEmpty(username))
        {
            query = query.Where(u => EF.Functions.Like(u.UserName, $"%{username}%"));
        }

        if (!string.IsNullOrEmpty(email))
        {
            query = query.Where(u => EF.Functions.Like(u.Email, $"%{email}%"));
        }

        var paginatedData = await query
            .Skip((page.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync()
            ;
        
        var response = new List<UserResponse>();
        
        foreach (var user in paginatedData)
        {
            var userResponse = new UserResponse(Guid.Parse(user.Id), user.DisplayName, user.UserName!, user.Email!, user.Active, await userManager.GetRolesAsync(user));
            response.Add(userResponse);
        }
        
        return TypedResults.Ok(new PaginatedResponse<UserResponse>
        {
            Items = response,
            CurrentPage = page.Value,
            PageSize = pageSize.Value,
            TotalPages = totalPages
        });
    }
}