namespace lab_01.DTOs.Booking
{
    public class BookingFilterDto
    {
        public string? RoomId { get; set; }

        public string? HotelId { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public string? SortBy { get; set; }

        public bool Descending { get; set; } = false;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}