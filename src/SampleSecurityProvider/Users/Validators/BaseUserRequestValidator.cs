using FluentValidation;
using SampleSecurityProvider.Users.Dtos;

namespace SampleSecurityProvider.Users.Validators;

public class BaseUserRequestValidator : AbstractValidator<BaseUserRequest>
{
    public BaseUserRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .WithErrorCode("users.validators.base-user-request.display-name-required")
            .WithMessage("O nome de exibição do usuário é obrigatório.");
        
        RuleFor(x => x.DisplayName)
            .MinimumLength(4)
            .WithErrorCode("users.validators.base-user-request.display-min-length")
            .WithMessage("O nome de exibição do usuário deve ter pelo menos 4 caracteres.");
        
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithErrorCode("users.validators.base-user-request.username-required")
            .WithMessage("O nome de usuário é obrigatório.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode("users.validators.base-user-request.email-required")
            .WithMessage("O email é obrigatório.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithErrorCode("users.validators.base-user-request.email-invalid-format")
            .WithMessage("O formato do email é invalido.");
    }
}