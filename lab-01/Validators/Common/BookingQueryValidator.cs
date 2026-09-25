using FluentValidation;
using lab_01.Repositories.Queries;
using lab_01.Validators.Common;

namespace lab_01.Validators.Booking
{
    public class BookingQueryValidator
        : BaseQueryValidator<BookingQuery>
    {
        private static readonly string[] AllowedSortFields =
        [
            "checkin",
            "checkout",
            "price",
            "createdat"
        ];

        public BookingQueryValidator()
        {
            RuleFor(x => x)
                .Must(query =>
                    !query.From.HasValue ||
                    !query.To.HasValue ||
                    query.To >= query.From
                )
                .WithMessage("Кінцева дата не може бути раніше початкової.");

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