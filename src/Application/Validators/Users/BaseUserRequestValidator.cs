using AuthHub.Application.Dtos.Users;
using FluentValidation;

namespace AuthHub.Application.Validators.Users;

public class BaseUserRequestValidator : AbstractValidator<BaseUserRequest>
{
    public BaseUserRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .WithErrorCode("application.validators.users.base-user-request.display-name-required")
            .WithMessage("O nome de exibição do usuário é obrigatório.");
        
        RuleFor(x => x.DisplayName)
            .MinimumLength(4)
            .WithErrorCode("application.validators.users.base-user-request.display-min-length")
            .WithMessage("O nome de exibição do usuário deve ter pelo menos 4 caracteres.");
        
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithErrorCode("application.validators.users.base-user-request.username-required")
            .WithMessage("O nome de usuário é obrigatório.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode("application.validators.users.base-user-request.email-required")
            .WithMessage("O email é obrigatório.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithErrorCode("application.validators.users.base-user-request.email-invalid-format")
            .WithMessage("O formato do email é invalido.");
    }
}