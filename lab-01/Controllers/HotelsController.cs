using FluentValidation;
using lab_01.DTOs.Hotel;
using lab_01.DTOs.Room;
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
        private readonly IRoomService _roomService;

        public HotelsController(
            IHotelService hotelService,
            IRoomService roomService)
        {
            _hotelService = hotelService;
            _roomService = roomService;
        }

        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> Details(
            string id,
            [FromQuery] RoomFilterDto filter)
        {
            HotelReadDto hotel =
                await _hotelService.GetByIdAsync(
                    id
                );

            filter.HotelId = id;
            filter.IsAvailable = true;

            HotelDetailsViewModel model =
                new HotelDetailsViewModel
                {
                    Hotel = hotel,
                    Filter = filter
                };

            try
            {
                model.Rooms =
                    await _roomService.SearchAsync(
                        filter
                    );
            }
            catch (ValidationException exception)
            {
                AddValidationErrors(
                    exception
                );
            }

            return View(model);
        }

        private void AddValidationErrors(
            ValidationException exception)
        {
            foreach (var error in exception.Errors)
            {
                ModelState.AddModelError(
                    error.PropertyName,
                    error.ErrorMessage
                );
            }
        }
    }
}