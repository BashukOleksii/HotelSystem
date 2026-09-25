using FluentValidation;
using lab_01.DTOs.Review;

namespace lab_01.Validators.Review
{
    public class ReviewCreateDtoValidator
        : AbstractValidator<ReviewCreateDto>
    {
        public ReviewCreateDtoValidator()
        {
            RuleFor(x => x.HotelId)
                .NotEmpty()
                .WithMessage("Готель є обов'язковим.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Оцінка повинна бути від 1 до 5.");

            RuleFor(x => x.Comment)
                .MaximumLength(2000)
                .WithMessage("Коментар не може перевищувати 2000 символів.")
                .When(x => x.Comment is not null);
        }
    }
}