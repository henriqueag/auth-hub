using MediatR;

namespace AuthHub.Application.Commands.Users.Delete;

public record DeleteCommand(Guid UserId) : IRequest;