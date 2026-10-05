using lab_01.Common.Pagination;
using lab_01.DTOs.Booking;

namespace lab_01.ViewModels.Bookings
{
    public class UserBookingIndexViewModel
    {
        public BookingFilterDto Filter { get; set; }
            = new();

        public PagedResult<BookingReadDto> Bookings { get; set; }
            = new();
    }
}