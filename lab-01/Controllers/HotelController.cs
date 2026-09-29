using lab_01.DTOs.Hotel;
using lab_01.Services.Interfaces;
using lab_01.ViewModels.Hotels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lab_01.Controllers
{
    [AllowAnonymous]
    public class HotelsController : Controller
    {
        private readonly IHotelService _hotelService;

        public HotelsController(
            IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        public async Task<IActionResult> Index(
            [FromQuery] HotelFilterDto filter)
        {
            var hotels =
                await _hotelService.SearchAsync(
                    filter
                );

            HotelIndexViewModel model =
                new HotelIndexViewModel
                {
                    Filter = filter,
                    Hotels = hotels
                };

            return View(model);
        }

        public async Task<IActionResult> Details(
            string id)
        {
            HotelReadDto hotel =
                await _hotelService.GetByIdAsync(
                    id
                );

            return View(hotel);
        }
    }
}