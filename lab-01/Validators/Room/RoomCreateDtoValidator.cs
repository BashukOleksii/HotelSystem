using FluentValidation;
using lab_01.DTOs.Room;

namespace lab_01.Validators.Room
{
    public class RoomCreateDtoValidator
        : AbstractValidator<RoomCreateDto>
    {
        public RoomCreateDtoValidator()
        {
            RuleFor(x => x.HotelId)
                .NotEmpty()
                .WithMessage("Ідентифікатор готелю є обов'язковим.");

            RuleFor(x => x.RoomNumber)
                .NotEmpty()
                .WithMessage("Номер кімнати є обов'язковим.")
                .MaximumLength(20)
                .WithMessage("Номер кімнати не може перевищувати 20 символів.");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Некоректний тип кімнати.");

            RuleFor(x => x.CostPerNight)
                .GreaterThan(0)
                .WithMessage("Вартість за ніч повинна бути більшою за 0.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .WithMessage("Місткість кімнати повинна бути більшою за 0.");
        }
    }
}