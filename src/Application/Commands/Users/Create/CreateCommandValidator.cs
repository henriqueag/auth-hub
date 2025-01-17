using AuthHub.Application.Validators.Users;
using FluentValidation;

namespace AuthHub.Application.Commands.Users.Create;

public class CreateCommandValidator : AbstractValidator<CreateCommand>
{
    public CreateCommandValidator()
    {
        RuleFor(request => request)
            .SetValidator(new BaseUserRequestValidator());
    }
}