using FluentValidation;
using lab_01.Repositories.Queries;
using lab_01.Validators.Common;

namespace lab_01.Validators.Hotel
{
    public class HotelQueryValidator
        : BaseQueryValidator<HotelQuery>
    {
        private static readonly string[] AllowedSortFields =
        [
            "name",
            "city",
            "createdat"
        ];

        public HotelQueryValidator()
        {
            RuleFor(x => x.City)
                .MaximumLength(100)
                .When(x => x.City is not null);

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