using FluentValidation;
using lab_01.DTOs.Booking;

namespace lab_01.Validators.Booking
{
    public class BookingUpdateDtoValidator
        : AbstractValidator<BookingUpdateDto>
    {
        public BookingUpdateDtoValidator()
        {
            RuleFor(x => x.CheckIn)
                .Must(date =>
                    !date.HasValue ||
                    date.Value >= DateTime.UtcNow.Date
                )
                .WithMessage("Дата заселення не може бути в минулому.");

            RuleFor(x => x)
                .Must(dto =>
                    !dto.CheckIn.HasValue ||
                    !dto.CheckOut.HasValue ||
                    dto.CheckOut.Value > dto.CheckIn.Value
                )
                .WithMessage("Дата виселення повинна бути пізнішою за дату заселення.");

            RuleFor(x => x)
                .Must(dto =>
                    dto.CheckIn.HasValue ||
                    dto.CheckOut.HasValue
                )
                .WithMessage("Потрібно вказати хоча б одну дату для оновлення.");
        }
    }
}