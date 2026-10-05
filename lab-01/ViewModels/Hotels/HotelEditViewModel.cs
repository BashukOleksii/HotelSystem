using lab_01.DTOs.Hotel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace lab_01.ViewModels.Hotels
{
    public class HotelEditViewModel
    {
        [ValidateNever]
        public HotelReadDto Hotel { get; set; }
            = null!;

        public HotelUpdateDto Form { get; set; }
            = new();

        [ValidateNever]
        public List<IFormFile> NewPhotos { get; set; }
            = [];
    }
}