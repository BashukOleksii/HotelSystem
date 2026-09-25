using lab_01.Enums;

namespace lab_01.DTOs.Room
{
    public class RoomUpdateDto
    {
        public string? RoomNumber { get; set; }
        public RoomType? Type { get; set; }
        public decimal? CostPerNight { get; set; }
        public int? Capacity { get; set; }
        public bool? IsAvailable { get; set; } = true;
    }
}
