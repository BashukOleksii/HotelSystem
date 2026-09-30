using FluentValidation;
using lab_01.DTOs.Auth;

namespace lab_01.Validators.Auth
{
    public class LoginDtoValidator
        : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email є обов'язковим.")
                .EmailAddress()
                .WithMessage("Некоректний формат Email.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Пароль є обов'язковим.");
        }
    }
}