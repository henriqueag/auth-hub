using AuthHub.Application.Dtos.Users;
using FluentValidation;

namespace AuthHub.Application.Validators.Users;

public class PasswordRequestValidator : AbstractValidator<PasswordRequest>
{
    public PasswordRequestValidator()
    {
        RuleFor(x => x.Password)
            .MinimumLength(6)
            .WithErrorCode("application.validators.users.password-request.password-min-length")
            .WithMessage("A senha precisa ter pelo menos 6 caracteres.");

        RuleFor(x => x.ConfirmPassword)
            .Must((req, curr) => req.Password.Equals(curr))
            .WithErrorCode("application.validators.users.password-request.confirm-password")
            .WithMessage("A senha e a confirmação de senha não correspondem.");
    }
}