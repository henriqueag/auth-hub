using FluentValidation;
using SampleSecurityProvider.Users.Dtos;

namespace SampleSecurityProvider.Users.Validators;

public class PasswordRequestValidator : AbstractValidator<PasswordRequest>
{
    public PasswordRequestValidator()
    {
        RuleFor(x => x.Password)
            .MinimumLength(6)
            .WithErrorCode("users.validators.password-request.password-min-length")
            .WithMessage("A senha precisa ter pelo menos 6 caracteres.");

        RuleFor(x => x.ConfirmPassword)
            .Must((req, curr) => req.Password.Equals(curr))
            .WithErrorCode("users.validators.password-request.confirm-password")
            .WithMessage("A senha e a confirmação de senha não correspondem.");
    }
}