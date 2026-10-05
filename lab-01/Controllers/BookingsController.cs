using System.Security.Claims;
using FluentValidation;
using lab_01.DTOs.Booking;
using lab_01.DTOs.Hotel;
using lab_01.DTOs.Room;
using lab_01.Exceptions;
using lab_01.Services.Interfaces;
using lab_01.ViewModels.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lab_01.Controllers
{
    [Authorize(Roles = "User")]
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IRoomService _roomService;
        private readonly IHotelService _hotelService;

        public BookingsController(
            IBookingService bookingService,
            IRoomService roomService,
            IHotelService hotelService)
        {
            _bookingService = bookingService;
            _roomService = roomService;
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] BookingFilterDto filter)
        {
            string userId =
                GetCurrentUserId();

            UserBookingIndexViewModel model =
                new UserBookingIndexViewModel
                {
                    Filter = filter
                };

            try
            {
                model.Bookings =
                    await _bookingService
                        .SearchUserBookingsAsync(
                            userId,
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

        [HttpGet]
        public async Task<IActionResult> Details(
            string id)
        {
            string userId =
                GetCurrentUserId();

            BookingReadDto booking =
                await _bookingService
                    .GetUserBookingByIdAsync(
                        id,
                        userId
                    );

            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> Create(
            string roomId,
            DateTime? checkIn,
            DateTime? checkOut)
        {
            DateTime startDate =
                checkIn?.Date ??
                DateTime.UtcNow.Date.AddDays(1);

            DateTime endDate =
                checkOut?.Date ??
                startDate.AddDays(1);

            BookingCreateDto form =
                new BookingCreateDto
                {
                    RoomId = roomId,
                    CheckIn = startDate,
                    CheckOut = endDate
                };

            BookingCreateViewModel model =
                await BuildCreateViewModelAsync(
                    form
                );

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            BookingCreateViewModel model)
        {
            try
            {
                string userId =
                    GetCurrentUserId();

                BookingReadDto booking =
                    await _bookingService
                        .CreateAsync(
                            userId,
                            model.Form
                        );

                TempData["Success"] =
                    "Бронювання успішно створено.";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = booking.Id
                    }
                );
            }
            catch (ValidationException exception)
            {
                AddValidationErrors(
                    exception,
                    "Form"
                );
            }
            catch (ConflictException exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    exception.Message
                );
            }

            model =
                await BuildCreateViewModelAsync(
                    model.Form
                );

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(
            string id)
        {
            string userId =
                GetCurrentUserId();

            BookingReadDto booking =
                await _bookingService
                    .GetUserBookingByIdAsync(
                        id,
                        userId
                    );

            BookingEditViewModel model =
                new BookingEditViewModel
                {
                    Booking = booking,

                    Form =
                        new BookingUpdateDto
                        {
                            CheckIn =
                                booking.CheckIn,

                            CheckOut =
                                booking.CheckOut
                        }
                };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            BookingEditViewModel model)
        {
            string userId =
                GetCurrentUserId();

            try
            {
                await _bookingService
                    .UpdateAsync(
                        id,
                        userId,
                        model.Form
                    );

                TempData["Success"] =
                    "Бронювання успішно оновлено.";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id
                    }
                );
            }
            catch (ValidationException exception)
            {
                AddValidationErrors(
                    exception,
                    "Form"
                );
            }
            catch (ConflictException exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    exception.Message
                );
            }

            model.Booking =
                await _bookingService
                    .GetUserBookingByIdAsync(
                        id,
                        userId
                    );

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(
            string id)
        {
            string userId =
                GetCurrentUserId();

            BookingReadDto booking =
                await _bookingService
                    .GetUserBookingByIdAsync(
                        id,
                        userId
                    );

            return View(booking);
        }

        [HttpPost]
        [ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            CancelConfirmed(
                string id)
        {
            string userId =
                GetCurrentUserId();

            await _bookingService.DeleteAsync(
                id,
                userId
            );

            TempData["Success"] =
                "Бронювання скасовано.";

            return RedirectToAction(
                nameof(Index)
            );
        }

        private async Task<BookingCreateViewModel>
            BuildCreateViewModelAsync(
                BookingCreateDto form)
        {
            RoomReadDto room =
                await _roomService.GetByIdAsync(
                    form.RoomId
                );

            HotelReadDto hotel =
                await _hotelService.GetByIdAsync(
                    room.HotelId
                );

            return new BookingCreateViewModel
            {
                Hotel = hotel,
                Room = room,
                Form = form
            };
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
            ValidationException exception,
            string? prefix = null)
        {
            foreach (var error in exception.Errors)
            {
                string key;

                if (string.IsNullOrWhiteSpace(
                    error.PropertyName))
                {
                    key = string.Empty;
                }
                else if (string.IsNullOrWhiteSpace(
                    prefix))
                {
                    key = error.PropertyName;
                }
                else
                {
                    key =
                        $"{prefix}.{error.PropertyName}";
                }

                ModelState.AddModelError(
                    key,
                    error.ErrorMessage
                );
            }
        }
    }
}