using System.Linq.Expressions;
using AuthHub.Application.Dtos;
using AuthHub.Application.Dtos.Users;
using AuthHub.Domain.Users.Entities;
using AuthHub.Domain.Users.Repositories;
using Bogus;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Queries.Users.GetAll;

public static class StaticGetUsers
{
    private static readonly IQueryable<User> s_users = new Faker<User>("pt_BR")
        .CustomInstantiator(faker => new User(faker.Person.FullName, faker.Person.UserName, faker.Person.Email)
        {
            EmailConfirmed = true,
            Active = true
        })
        .Generate(30)
        .AsQueryable();

    public static IQueryable<User> Users { get; } = s_users;
}

public class GetAllQueryHandler(IUserRepository userRepository, UserManager<User> userManager) : IRequestHandler<GetAllQuery, PagedResponse<UserResponse>>
{
    private static IQueryable<User> Users => StaticGetUsers.Users;
    
    public async Task<PagedResponse<UserResponse>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var skip = request.Skip ?? 0;
        var limit = request.Limit ?? 10;
        
        // var totalItems = await userRepository.CountAsync(cancellationToken);
        // var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        //
        // var paginatedData = await userRepository.GetAllAsync(page, pageSize, request.DisplayName, request.Username, request.Email, cancellationToken);
        //
        // var response = new List<UserResponse>();
        //
        // foreach (var user in paginatedData)
        // {
        //     var userResponse = new UserResponse(Guid.Parse(user.Id), user.DisplayName, user.UserName!, user.Email!, user.Active, await userManager.GetRolesAsync(user));
        //     response.Add(userResponse);
        // }
        
        var paginatedData = GetAll(skip, limit, request.Query);
        
        var response = new List<UserResponse>();
        
        foreach (var user in paginatedData)
        {
            var userResponse = new UserResponse(Guid.Parse(user.Id), user.DisplayName, user.UserName!, user.Email!, user.Active, await userManager.GetRolesAsync(user));
            response.Add(userResponse);
        }

        return new PagedResponse<UserResponse>
        {
            Items = response,
            TotalRecords = Users.Count()
        };
    }
    
    private static List<User> GetAll(int skip, int limit, string? query)
    {
        var queryable = Users.AsQueryable();

        if (!string.IsNullOrEmpty(query))
        {
            queryable = queryable.Where(user => 
                user.DisplayName.Contains(query, StringComparison.InvariantCultureIgnoreCase) ||
                user.UserName!.Contains(query, StringComparison.InvariantCultureIgnoreCase) ||
                user.Email!.Contains(query, StringComparison.InvariantCultureIgnoreCase)
            );
        }

        queryable = queryable.OrderBy(user => user.DisplayName);
        
        if (skip > 0)
        {
            queryable = queryable.Skip(skip);
        }

        return queryable.Take(limit).ToList();
    }
}