using lab_01.DTOs.Booking;
using lab_01.DTOs.Hotel;
using lab_01.DTOs.Room;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace lab_01.ViewModels.Bookings
{
    public class BookingCreateViewModel
    {
        [ValidateNever]
        public HotelReadDto Hotel { get; set; }
            = null!;

        [ValidateNever]
        public RoomReadDto Room { get; set; }
            = null!;

        public BookingCreateDto Form { get; set; }
            = new();
    }
}