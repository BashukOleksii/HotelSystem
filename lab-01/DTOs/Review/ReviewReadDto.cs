using lab_01.DTOs.Base;

namespace lab_01.DTOs.Review
{
    public class ReviewReadDto : BaseReadDto
    {
        public string HotelId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
