using FluentValidation;
using lab_01.DTOs.Hotel;

namespace lab_01.Validators.Hotel
{
    public class HotelUpdateDtoValidator
        : AbstractValidator<HotelUpdateDto>
    {
        public HotelUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Назва готелю не може бути порожньою.")
                .MinimumLength(2)
                .MaximumLength(100)
                .When(x => x.Name is not null);

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Опис готелю не може перевищувати 2000 символів.")
                .When(x => x.Description is not null);

            When(x => x.Address is not null, () =>
            {
                RuleFor(x => x.Address!)
                    .SetValidator(new AddressDtoValidator());
            });

            RuleFor(x => x)
                .Must(dto =>
                    dto.Name is not null ||
                    dto.Description is not null ||
                    dto.Address is not null
                )
                .WithMessage("Потрібно вказати хоча б одне поле для оновлення.");
        }
    }
}