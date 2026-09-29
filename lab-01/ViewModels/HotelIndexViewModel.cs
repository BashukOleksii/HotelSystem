using lab_01.Common.Pagination;
using lab_01.DTOs.Hotel;

namespace lab_01.ViewModels.Hotels
{
    public class HotelIndexViewModel
    {
        public HotelFilterDto Filter { get; set; }
            = new();

        public PagedResult<HotelReadDto> Hotels { get; set; }
            = new();
    }
}