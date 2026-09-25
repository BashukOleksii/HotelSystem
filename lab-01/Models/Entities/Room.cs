using lab_01.Enums;
using lab_01.Models.Entities.Base;

namespace lab_01.Models.Entities
{
    public class Room : BaseEntry
    {
        public string HotelId { get; set; }
        public string RoomNumber { get; set; }
        public RoomType Type { get; set; } = RoomType.Single;
        public decimal CostPerNight { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; } = true;

        public Hotel Hotel { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
