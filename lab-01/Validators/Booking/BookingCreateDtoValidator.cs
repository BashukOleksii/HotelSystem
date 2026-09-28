using FluentValidation;
using lab_01.DTOs.Booking;

namespace lab_01.Validators.Booking
{
    public class BookingCreateDtoValidator
        : AbstractValidator<BookingCreateDto>
    {
        public BookingCreateDtoValidator()
        {
            RuleFor(x => x.RoomId)
                .NotEmpty()
                .WithMessage("Кімната є обов'язковою.");

            RuleFor(x => x.CheckIn)
                 .Must(date => date.Date >= DateTime.UtcNow.Date)
                 .WithMessage("Дата заселення не може бути в минулому.");

            RuleFor(x => x)
                .Must(dto =>
                    dto.CheckOut.Date > dto.CheckIn.Date
                )
                .WithMessage(
                    "Дата виселення повинна бути пізнішою за дату заселення."
                );
        }
    }
}