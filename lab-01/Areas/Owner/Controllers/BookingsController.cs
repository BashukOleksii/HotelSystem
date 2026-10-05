using System.Security.Claims;
using FluentValidation;
using lab_01.Common.Pagination;
using lab_01.DTOs.Booking;
using lab_01.DTOs.Hotel;
using lab_01.Services.Interfaces;
using lab_01.ViewModels.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lab_01.Areas.Owner.Controllers
{
    [Area("Owner")]
    [Authorize(Roles = "Owner")]
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IHotelService _hotelService;

        public BookingsController(
            IBookingService bookingService,
            IHotelService hotelService)
        {
            _bookingService = bookingService;
            _hotelService = hotelService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] BookingFilterDto filter)
        {
            string ownerId =
                GetCurrentUserId();

            var ownerHotels =
                await _hotelService
                    .SearchOwnerHotelsAsync(
                        ownerId,
                        new HotelFilterDto
                        {
                            Page = 1,
                            PageSize = 100,
                            SortBy = "name"
                        }
                    );

            try
            {
                PagedResult<BookingReadDto> bookings =
                    await _bookingService
                        .SearchOwnerBookingsAsync(
                            ownerId,
                            filter
                        );

                OwnerBookingIndexViewModel model =
                    new OwnerBookingIndexViewModel
                    {
                        Filter = filter,
                        Bookings = bookings,
                        Hotels = ownerHotels.Items
                    };

                return View(model);
            }
            catch (ValidationException exception)
            {
                AddValidationErrors(
                    exception
                );

                OwnerBookingIndexViewModel model =
                    new OwnerBookingIndexViewModel
                    {
                        Filter = filter,

                        Bookings =
                            new PagedResult<BookingReadDto>(),

                        Hotels = ownerHotels.Items
                    };

                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Details(
            string id)
        {
            string ownerId =
                GetCurrentUserId();

            BookingReadDto booking =
                await _bookingService
                    .GetOwnerBookingByIdAsync(
                        id,
                        ownerId
                    );

            return View(booking);
        }


        private string GetCurrentUserId()
        {
            string? userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

            if (string.IsNullOrWhiteSpace(
                userId))
            {
                throw new UnauthorizedAccessException(
                    "Не вдалося визначити поточного користувача."
                );
            }

            return userId;
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