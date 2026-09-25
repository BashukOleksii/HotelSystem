using FluentValidation;
using lab_01.Common.Queries;

namespace lab_01.Validators.Common
{
    public abstract class BaseQueryValidator<TQuery>
        : AbstractValidator<TQuery>
        where TQuery : BaseQuery
    {
        protected BaseQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Номер сторінки повинен бути не менше 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Розмір сторінки повинен бути від 1 до 100.");

            RuleFor(x => x.Search)
                .MaximumLength(200)
                .WithMessage("Рядок пошуку не може перевищувати 200 символів.")
                .When(x => x.Search is not null);

            RuleFor(x => x.SortBy)
                .MaximumLength(50)
                .When(x => x.SortBy is not null);
        }
    }
}