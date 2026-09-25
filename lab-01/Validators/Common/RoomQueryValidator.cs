using FluentValidation;
using lab_01.Repositories.Queries;
using lab_01.Validators.Common;

namespace lab_01.Validators.Room
{
    public class RoomQueryValidator
        : BaseQueryValidator<RoomQuery>
    {
        private static readonly string[] AllowedSortFields =
        [
            "number",
            "price",
            "capacity",
            "type",
            "createdat"
        ];

        public RoomQueryValidator()
        {
            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinPrice.HasValue);

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxPrice.HasValue);

            RuleFor(x => x.MinCapacity)
                .GreaterThan(0)
                .When(x => x.MinCapacity.HasValue);

            RuleFor(x => x)
                .Must(query =>
                    !query.MinPrice.HasValue ||
                    !query.MaxPrice.HasValue ||
                    query.MaxPrice >= query.MinPrice
                )
                .WithMessage("Максимальна ціна не може бути меншою за мінімальну.");

            RuleFor(x => x)
                .Must(query =>
                    query.CheckIn.HasValue ==
                    query.CheckOut.HasValue
                )
                .WithMessage("Для пошуку за датами потрібно вказати і дату заселення, і дату виселення.");

            RuleFor(x => x)
                .Must(query =>
                    !query.CheckIn.HasValue ||
                    !query.CheckOut.HasValue ||
                    query.CheckOut > query.CheckIn
                )
                .WithMessage("Дата виселення повинна бути пізнішою за дату заселення.");

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