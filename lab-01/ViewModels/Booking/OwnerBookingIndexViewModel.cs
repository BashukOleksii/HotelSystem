using lab_01.Common.Pagination;
using lab_01.DTOs.Booking;
using lab_01.DTOs.Hotel;

namespace lab_01.ViewModels.Bookings
{
    public class OwnerBookingIndexViewModel
    {
        public BookingFilterDto Filter { get; set; }
            = new();

        public PagedResult<BookingReadDto> Bookings { get; set; }
            = new();

        public IReadOnlyList<HotelReadDto> Hotels { get; set; }
            = [];
    }
}