using System.Security.Claims;
using FluentValidation;
using lab_01.DTOs.Hotel;
using lab_01.DTOs.Room;
using lab_01.Exceptions;
using lab_01.Services.Interfaces;
using lab_01.ViewModels.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lab_01.Areas.Owner.Controllers
{
    [Area("Owner")]
    [Authorize(Roles = "Owner")]
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IHotelService _hotelService;

        public RoomsController(
            IRoomService roomService,
            IHotelService hotelService)
        {
            _roomService = roomService;
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string hotelId,
            [FromQuery] RoomFilterDto filter)
        {
            string ownerId =
                GetCurrentUserId();

            HotelReadDto hotel =
                await _hotelService
                    .GetOwnerHotelByIdAsync(
                        hotelId,
                        ownerId
                    );

            var rooms =
                await _roomService
                    .SearchOwnerHotelRoomsAsync(
                        ownerId,
                        hotelId,
                        filter
                    );

            OwnerRoomIndexViewModel model =
                new OwnerRoomIndexViewModel
                {
                    Hotel = hotel,
                    Filter = filter,
                    Rooms = rooms
                };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Details(
            string id)
        {
            string ownerId =
                GetCurrentUserId();

            RoomReadDto room =
                await _roomService
                    .GetOwnerRoomByIdAsync(
                        id,
                        ownerId
                    );

            return View(room);
        }

        [HttpGet]
        public async Task<IActionResult> Create(
            string hotelId)
        {
            string ownerId =
                GetCurrentUserId();

            await _hotelService
                .GetOwnerHotelByIdAsync(
                    hotelId,
                    ownerId
                );

            RoomCreateDto dto =
                new RoomCreateDto
                {
                    HotelId = hotelId
                };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            RoomCreateDto dto)
        {
            try
            {
                string ownerId =
                    GetCurrentUserId();

                await _roomService.CreateAsync(
                    ownerId,
                    dto
                );

                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        hotelId = dto.HotelId
                    }
                );
            }
            catch (ValidationException exception)
            {
                AddValidationErrors(
                    exception
                );

                return View(dto);
            }
            catch (ConflictException exception)
            {
                ModelState.AddModelError(
                    nameof(dto.RoomNumber),
                    exception.Message
                );

                return View(dto);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(
            string id)
        {
            string ownerId =
                GetCurrentUserId();

            RoomReadDto room =
                await _roomService
                    .GetOwnerRoomByIdAsync(
                        id,
                        ownerId
                    );

            RoomUpdateDto dto =
                new RoomUpdateDto
                {
                    RoomNumber =
                        room.RoomNumber,

                    Type =
                        room.Type,

                    CostPerNight =
                        room.CostPerNight,

                    Capacity =
                        room.Capacity,

                    IsAvailable =
                        room.IsAvailable
                };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            RoomUpdateDto dto)
        {
            try
            {
                string ownerId =
                    GetCurrentUserId();

                RoomReadDto room =
                    await _roomService.UpdateAsync(
                        id,
                        ownerId,
                        dto
                    );

                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        hotelId = room.HotelId
                    }
                );
            }
            catch (ValidationException exception)
            {
                AddValidationErrors(
                    exception
                );

                return View(dto);
            }
            catch (ConflictException exception)
            {
                ModelState.AddModelError(
                    nameof(dto.RoomNumber),
                    exception.Message
                );

                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(
            string id)
        {
            string ownerId =
                GetCurrentUserId();

            RoomReadDto room =
                await _roomService
                    .GetOwnerRoomByIdAsync(
                        id,
                        ownerId
                    );

            return View(room);
        }


        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            DeleteConfirmed(
                string id)
        {
            string ownerId =
                GetCurrentUserId();

            RoomReadDto room =
                await _roomService
                    .GetOwnerRoomByIdAsync(
                        id,
                        ownerId
                    );

            await _roomService.DeleteAsync(
                id,
                ownerId
            );

            return RedirectToAction(
                nameof(Index),
                new
                {
                    hotelId = room.HotelId
                }
            );
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