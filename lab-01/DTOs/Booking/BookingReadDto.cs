using lab_01.DTOs.Base;

namespace lab_01.DTOs.Booking
{
    public class BookingReadDto : BaseReadDto
    {
        public string UserId { get; set; }
        public string RoomId { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; private set; }
    }
}

