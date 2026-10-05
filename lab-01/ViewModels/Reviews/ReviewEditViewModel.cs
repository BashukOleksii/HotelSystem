using lab_01.DTOs.Review;

namespace lab_01.ViewModels.Reviews
{
    public class ReviewEditViewModel
    {
        public ReviewReadDto Review { get; set; }
            = null!;

        public ReviewUpdateDto Form { get; set; }
            = new();
    }
}