using lab_01.DTOs.Booking;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace lab_01.ViewModels.Bookings
{
    public class BookingEditViewModel
    {
        [ValidateNever]
        public BookingReadDto Booking { get; set; }
            = null!;

        public BookingUpdateDto Form { get; set; }
            = new();
    }
}