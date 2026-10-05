using lab_01.DTOs.Booking;
using lab_01.DTOs.Hotel;
using lab_01.DTOs.Room;

namespace lab_01.ViewModels.Bookings
{
    public class BookingCreateViewModel
    {
        public HotelReadDto Hotel { get; set; }
            = null!;

        public RoomReadDto Room { get; set; }
            = null!;

        public BookingCreateDto Form { get; set; }
            = new();
    }
}