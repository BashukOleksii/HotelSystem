using lab_01.DTOs.Booking;

namespace lab_01.ViewModels.Bookings
{
    public class BookingEditViewModel
    {
        public BookingReadDto Booking { get; set; }
            = null!;

        public BookingUpdateDto Form { get; set; }
            = new();
    }
}