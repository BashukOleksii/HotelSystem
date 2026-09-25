using lab_01.Models.Entities;

namespace lab_01.DTOs.Review
{
    public class ReviewCreateDto
    {
        public string HotelId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
