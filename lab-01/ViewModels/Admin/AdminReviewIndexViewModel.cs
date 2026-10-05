using lab_01.Common.Pagination;
using lab_01.DTOs.Review;

namespace lab_01.ViewModels.Admin
{
    public class AdminReviewIndexViewModel
    {
        public ReviewFilterDto Filter { get; set; }
            = new();

        public PagedResult<ReviewReadDto> Reviews { get; set; }
            = new();
    }
}