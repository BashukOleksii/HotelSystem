using System.Security.Claims;
using FluentValidation;
using lab_01.DTOs.Hotel;
using lab_01.DTOs.Hotel.Address;
using lab_01.Services.Interfaces;
using lab_01.ViewModels.Hotels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lab_01.Areas.Owner.Controllers
{
    [Area("Owner")]
    [Authorize(Roles = "Owner")]
    public class HotelsController : Controller
    {
        private readonly IHotelService _hotelService;

        public HotelsController(
            IHotelService hotelService)
        {
            _hotelService = hotelService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] HotelFilterDto filter)
        {
            string ownerId =
                GetCurrentUserId();

            var hotels =
                await _hotelService
                    .SearchOwnerHotelsAsync(
                        ownerId,
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
            string id)
        {
            string ownerId =
                GetCurrentUserId();

            HotelReadDto hotel =
                await _hotelService
                    .GetOwnerHotelByIdAsync(
                        id,
                        ownerId
                    );

            return View(hotel);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(
                new HotelCreateDto
                {
                    Address =
                        new AddressDto()
                }
            );
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            HotelCreateDto dto)
        {
            try
            {
                string ownerId =
                    GetCurrentUserId();

                await _hotelService.CreateAsync(
                    ownerId,
                    dto
                );

                return RedirectToAction(
                    nameof(Index)
                );
            }
            catch (ValidationException exception)
            {
                AddValidationErrors(
                    exception
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

            HotelReadDto hotel =
                await _hotelService
                    .GetOwnerHotelByIdAsync(
                        id,
                        ownerId
                    );


            HotelUpdateDto dto =
                new HotelUpdateDto
                {
                    Name =
                        hotel.Name,

                    Description =
                        hotel.Description,

                    Address =
                        new AddressDto
                        {
                            City =
                                hotel.Address.City,

                            Street =
                                hotel.Address.Street,

                            Number =
                                hotel.Address.Number
                        }
                };


            return View(dto);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            HotelUpdateDto dto)
        {
            try
            {
                string ownerId =
                    GetCurrentUserId();

                await _hotelService.UpdateAsync(
                    id,
                    ownerId,
                    dto
                );


                return RedirectToAction(
                    nameof(Index)
                );
            }
            catch (ValidationException exception)
            {
                AddValidationErrors(
                    exception
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

            HotelReadDto hotel =
                await _hotelService
                    .GetOwnerHotelByIdAsync(
                        id,
                        ownerId
                    );

            return View(hotel);
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

            await _hotelService.DeleteAsync(
                id,
                ownerId
            );

            return RedirectToAction(
                nameof(Index)
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