using AuthHub.Application.Dtos.Users;
using MediatR;

namespace AuthHub.Application.Queries.Users.GetUserById;

public record GetByIdQuery(Guid UserId) : IRequest<UserResponse>;