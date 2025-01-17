using AuthHub.Application.Dtos;
using AuthHub.Application.Dtos.Users;
using MediatR;

namespace AuthHub.Application.Queries.Users.GetAll;

public record GetAllQuery(int? Page, int? PageSize, string? DisplayName, string? Username, string? Email) : IRequest<PaginatedResponse<UserResponse>>;