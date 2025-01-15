using FluentValidation;
using SampleSecurityProvider.Users.Dtos;

namespace SampleSecurityProvider.Users.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(request => request)
            .SetValidator(new BaseUserRequestValidator());
    }
}