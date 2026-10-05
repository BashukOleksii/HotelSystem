using FluentValidation;
using lab_01.DTOs.Hotel;
using lab_01.DTOs.Review;
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
        private readonly IReviewService _reviewService;

        public HotelsController(
            IHotelService hotelService,
            IRoomService roomService,
            IReviewService reviewService)
        {
            _hotelService =
                hotelService;

            _roomService =
                roomService;

            _reviewService =
                reviewService;
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

            filter.HotelId =
                id;

            filter.IsAvailable =
                true;

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

            model.Reviews =
                await _reviewService.SearchAsync(
                    new ReviewFilterDto
                    {
                        HotelId = id,
                        SortBy = "createdat",
                        Descending = true,
                        Page = 1,
                        PageSize = 10
                    }
                );

            model.AverageRating =
                await _reviewService
                    .GetAverageRatingAsync(
                        id
                    );

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