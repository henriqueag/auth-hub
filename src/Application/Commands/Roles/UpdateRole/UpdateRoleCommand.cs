using AuthHub.Application.Dtos.Roles;
using MediatR;

namespace AuthHub.Application.Commands.Roles.UpdateRole;

public record UpdateRoleCommand(Guid Id, RoleRequest Payload) : IRequest;