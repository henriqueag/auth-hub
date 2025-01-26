using AuthHub.Application.Dtos;
using AuthHub.Application.Dtos.Users;
using MediatR;

namespace AuthHub.Application.Queries.Users.GetAll;

public record GetAllQuery(int? Skip, int? Limit, string? Query) : IRequest<PagedResponse<UserResponse>>;