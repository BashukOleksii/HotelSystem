namespace lab_01.DTOs.Booking
{
    public class BookingCreateDto
    {
        public string RoomId { get; set; }
        public DateTime CheckIn { get; set; } = DateTime.UtcNow;
        public DateTime CheckOut { get; set; }
    }
}
