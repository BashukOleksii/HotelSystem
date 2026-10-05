using lab_01.DTOs.Hotel;
using lab_01.DTOs.Review;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace lab_01.ViewModels.Reviews
{
    public class ReviewCreateViewModel
    {
        [ValidateNever]
        public HotelReadDto Hotel { get; set; }
            = null!;

        public ReviewCreateDto Form { get; set; }
            = new();
    }
}