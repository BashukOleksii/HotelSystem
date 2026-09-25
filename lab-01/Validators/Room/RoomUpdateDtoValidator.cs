using FluentValidation;
using lab_01.DTOs.Room;

namespace lab_01.Validators.Room
{
    public class RoomUpdateDtoValidator
        : AbstractValidator<RoomUpdateDto>
    {
        public RoomUpdateDtoValidator()
        {
            RuleFor(x => x.RoomNumber)
                .NotEmpty()
                .MaximumLength(20)
                .When(x => x.RoomNumber is not null);

            RuleFor(x => x.Type)
                .IsInEnum()
                .When(x => x.Type.HasValue);

            RuleFor(x => x.CostPerNight)
                .GreaterThan(0)
                .When(x => x.CostPerNight.HasValue);

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .When(x => x.Capacity.HasValue);

            RuleFor(x => x)
                .Must(dto =>
                    dto.RoomNumber is not null ||
                    dto.Type.HasValue ||
                    dto.CostPerNight.HasValue ||
                    dto.Capacity.HasValue ||
                    dto.IsAvailable.HasValue
                )
                .WithMessage("Потрібно вказати хоча б одне поле для оновлення.");
        }
    }
}