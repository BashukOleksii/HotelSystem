using FluentValidation;
using lab_01.DTOs.Review;

namespace lab_01.Validators.Review
{
    public class ReviewUpdateDtoValidator
        : AbstractValidator<ReviewUpdateDto>
    {
        public ReviewUpdateDtoValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .When(x => x.Rating.HasValue);

            RuleFor(x => x.Comment)
                .MaximumLength(2000)
                .When(x => x.Comment is not null);

            RuleFor(x => x)
                .Must(dto =>
                    dto.Rating.HasValue ||
                    dto.Comment is not null
                )
                .WithMessage("Потрібно вказати хоча б одне поле для оновлення.");
        }
    }
}