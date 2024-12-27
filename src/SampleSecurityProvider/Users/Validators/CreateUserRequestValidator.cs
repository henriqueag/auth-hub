using FluentValidation;
using SampleSecurityProvider.Users.Dtos;

namespace SampleSecurityProvider.Users.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request)
            .SetValidator(new BaseUserRequestValidator());
        
        RuleFor(x => x.Password)
            .MinimumLength(6)
            .WithErrorCode("users.validators.create-user-request.password-min-length")
            .WithMessage("A senha precisa ter pelo menos 6 caracteres.");

        RuleFor(x => x.ConfirmPassword)
            .Must((req, curr) => req.Password.Equals(curr))
            .WithErrorCode("users.validators.create-user-request.confirm-password")
            .WithMessage("A senha e a confirmação de senha não correspondem.");
    }
}