using lab_01.DTOs.Hotel;
using lab_01.DTOs.Review;

namespace lab_01.ViewModels.Reviews
{
    public class ReviewCreateViewModel
    {
        public HotelReadDto Hotel { get; set; }
            = null!;

        public ReviewCreateDto Form { get; set; }
            = new();
    }
}