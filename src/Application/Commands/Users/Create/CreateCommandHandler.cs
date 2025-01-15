using MediatR;

namespace AuthHub.Application.Commands.Users.Create;

public class CreateCommandHandler : IRequestHandler<CreateCommand>
{
    public Task Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}