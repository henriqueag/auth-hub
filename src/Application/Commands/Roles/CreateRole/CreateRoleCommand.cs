using AuthHub.Application.Dtos.Roles;
using MediatR;

namespace AuthHub.Application.Commands.Roles.CreateRole;

public record CreateRoleCommand(RoleRequest Payload) : IRequest<string>;