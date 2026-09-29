using FluentValidation;
using lab_01.DTOs.User;

namespace lab_01.Validators.User
{
    public class UserUpdateDtoValidator
        : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .MaximumLength(100)
                .When(x => x.UserName is not null);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(256)
                .When(x => x.Email is not null);

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(30)
                .When(x => x.PhoneNumber is not null);

            RuleFor(x => x)
                .Must(dto =>
                    dto.UserName is not null ||
                    dto.Email is not null ||
                    dto.PhoneNumber is not null
                )
                .WithMessage(
                    "Потрібно вказати хоча б одне поле для оновлення."
                );
        }
    }
}