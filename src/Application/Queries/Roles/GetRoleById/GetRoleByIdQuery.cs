using MediatR;

namespace AuthHub.Application.Queries.Roles.GetRoleById;

public record GetRoleByIdQuery(Guid Id) : IRequest<object>;