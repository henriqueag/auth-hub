using AuthHub.Domain.Users.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Commands.Users.Delete;

public class DeleteCommandHandler(UserManager<User> userManager) : IRequestHandler<DeleteCommand>
{
    public async Task Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return;
        }

        await userManager.DeleteAsync(user);
    }
}