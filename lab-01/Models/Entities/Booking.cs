using lab_01.Models.Entities.Base;

namespace lab_01.Models.Entities
{
    public class Booking : BaseEntry
    {
        public string? UserId { get; set; }
        public string RoomId { get; set; }

        public DateTime CheckIn { get; set; } = DateTime.UtcNow;
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }

        public User User { get; set; }
        public Room Room { get; set; }

    }
}
