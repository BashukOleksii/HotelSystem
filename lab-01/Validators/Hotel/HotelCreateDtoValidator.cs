using FluentValidation;
using lab_01.DTOs.Hotel;

namespace lab_01.Validators.Hotel
{
    public class HotelCreateDtoValidator
        : AbstractValidator<HotelCreateDto>
    {
        public HotelCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Назва готелю є обов'язковою.")
                .MinimumLength(2)
                .WithMessage("Назва готелю повинна містити щонайменше 2 символи.")
                .MaximumLength(100)
                .WithMessage("Назва готелю не може перевищувати 100 символів.");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Опис готелю не може перевищувати 2000 символів.")
                .When(x => x.Description is not null);

            RuleFor(x => x.Address)
                .NotNull()
                .WithMessage("Адреса готелю є обов'язковою.")
                .SetValidator(new AddressDtoValidator());
        }
    }
}