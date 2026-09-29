using FluentValidation;
using lab_01.DTOs.User;

namespace lab_01.Validators.User
{
    public class UserFilterDtoValidator
        : AbstractValidator<UserFilterDto>
    {
        private static readonly string[] AllowedSortFields =
        [
            "username",
            "email",
            "createdat"
        ];

        public UserFilterDtoValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage(
                    "Номер сторінки повинен бути не менше 1."
                );

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage(
                    "Розмір сторінки повинен бути від 1 до 100."
                );

            RuleFor(x => x.Search)
                .MaximumLength(200)
                .When(x => x.Search is not null);

            RuleFor(x => x.SortBy)
                .Must(sortBy =>
                    sortBy is null ||
                    AllowedSortFields.Contains(
                        sortBy.ToLowerInvariant()
                    )
                )
                .WithMessage(
                    "Некоректне поле сортування."
                );
        }
    }
}