using lab_01.DTOs.Review;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace lab_01.ViewModels.Reviews
{
    public class ReviewEditViewModel
    {
        [ValidateNever]
        public ReviewReadDto Review { get; set; }
            = null!;

        public ReviewUpdateDto Form { get; set; }
            = new();
    }
}