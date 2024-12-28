using FluentValidation;
using SampleSecurityProvider.Users.Dtos;

namespace SampleSecurityProvider.Users.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request)
            .SetValidator(new BaseUserRequestValidator());
    }
}