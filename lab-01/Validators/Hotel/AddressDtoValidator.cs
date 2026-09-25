using FluentValidation;
using lab_01.DTOs.Hotel.Address;

namespace lab_01.Validators.Hotel
{
    public class AddressDtoValidator : AbstractValidator<AddressDto>
    {
        public AddressDtoValidator()
        {
            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("Місто є обов'язковим.")
                .MaximumLength(100)
                .WithMessage("Назва міста не може перевищувати 100 символів.");

            RuleFor(x => x.Street)
                .NotEmpty()
                .WithMessage("Вулиця є обов'язковою.")
                .MaximumLength(100)
                .WithMessage("Назва вулиці не може перевищувати 100 символів.");

            RuleFor(x => x.Number)
                .NotEmpty()
                .WithMessage("Номер будинку є обов'язковим.")
                .MaximumLength(20)
                .WithMessage("Номер будинку не може перевищувати 20 символів.");
        }
    }
}