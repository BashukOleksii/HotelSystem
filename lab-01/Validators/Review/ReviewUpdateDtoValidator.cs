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
                .When(x =>
                    x.Rating.HasValue
                );

            RuleFor(x => x.Comment)
                .MaximumLength(2000)
                .When(x =>
                    x.Comment is not null
                );

            RuleFor(x => x.NewPhotoUrls)
                .Must(urls =>
                    urls.Count <= 5)
                .WithMessage(
                    "За один раз можна додати не більше 5 фотографій."
                );

            RuleFor(x => x)
                .Must(dto =>
                    dto.Rating.HasValue ||
                    dto.Comment is not null ||
                    dto.NewPhotoUrls.Count > 0
                )
                .WithMessage(
                    "Потрібно вказати хоча б одне поле для оновлення."
                );
        }
    }
}