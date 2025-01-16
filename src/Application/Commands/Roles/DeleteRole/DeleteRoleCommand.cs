using MediatR;

namespace AuthHub.Application.Commands.Roles.DeleteRole;

public record DeleteRoleCommand(Guid Id) : IRequest;