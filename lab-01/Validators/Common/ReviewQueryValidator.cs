using FluentValidation;
using lab_01.Repositories.Queries;
using lab_01.Validators.Common;

namespace lab_01.Validators.Review
{
    public class ReviewQueryValidator
        : BaseQueryValidator<ReviewQuery>
    {
        private static readonly string[] AllowedSortFields =
        [
            "rating",
            "createdat"
        ];

        public ReviewQueryValidator()
        {
            RuleFor(x => x.MinRating)
                .InclusiveBetween(1, 5)
                .When(x => x.MinRating.HasValue);

            RuleFor(x => x.MaxRating)
                .InclusiveBetween(1, 5)
                .When(x => x.MaxRating.HasValue);

            RuleFor(x => x)
                .Must(query =>
                    !query.MinRating.HasValue ||
                    !query.MaxRating.HasValue ||
                    query.MaxRating >= query.MinRating
                )
                .WithMessage("Максимальний рейтинг не може бути меншим за мінімальний.");

            RuleFor(x => x.SortBy)
                .Must(sortBy =>
                    sortBy is null ||
                    AllowedSortFields.Contains(
                        sortBy.ToLowerInvariant()
                    )
                )
                .WithMessage("Некоректне поле сортування.");
        }
    }
}