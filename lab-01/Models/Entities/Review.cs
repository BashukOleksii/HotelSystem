using lab_01.Models.Entities.Base;

namespace lab_01.Models.Entities
{
    public class Review : BaseEntry
    {
        public string HotelId { get; set; }
        public string? UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        public Hotel Hotel { get; set; }
        public User User { get; set; }
    }
}
