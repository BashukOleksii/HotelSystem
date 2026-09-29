using FluentValidation;
using lab_01.DTOs.User;

namespace lab_01.Validators.User
{
    public class UserCreateDtoValidator
        : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Ім'я користувача є обов'язковим.")
                .MaximumLength(100)
                .WithMessage("Ім'я користувача не може перевищувати 100 символів.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email є обов'язковим.")
                .EmailAddress()
                .WithMessage("Некоректний формат Email.")
                .MaximumLength(256);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Пароль є обов'язковим.")
                .MinimumLength(8)
                .WithMessage("Пароль повинен містити щонайменше 8 символів.")
                .Matches("[A-Z]")
                .WithMessage("Пароль повинен містити велику літеру.")
                .Matches("[a-z]")
                .WithMessage("Пароль повинен містити малу літеру.")
                .Matches("[0-9]")
                .WithMessage("Пароль повинен містити цифру.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(30)
                .When(x => x.PhoneNumber is not null);
        }
    }
}