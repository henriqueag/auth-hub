using AuthHub.Application.Validators.Users;
using FluentValidation;

namespace AuthHub.Application.Commands.Users.Update;

public class UpdateCommandValidator : AbstractValidator<UpdateCommand>
{
    public UpdateCommandValidator()
    {
        RuleFor(command => command)
            .SetValidator(new BaseUserRequestValidator());
    }
}