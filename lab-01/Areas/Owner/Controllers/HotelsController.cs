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
        private readonly IImageStorageService _imageStorageService;

        public HotelsController(
            IHotelService hotelService,
            IImageStorageService imageStorageService)
        {
            _hotelService =
                hotelService;

            _imageStorageService =
                imageStorageService;
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

            return View(
                new HotelIndexViewModel
                {
                    Filter = filter,
                    Hotels = hotels
                }
            );
        }

        [HttpGet]
        public async Task<IActionResult> Details(
            string id)
        {
            string ownerId =
                GetCurrentUserId();

            return View(
                await _hotelService
                    .GetOwnerHotelByIdAsync(
                        id,
                        ownerId
                    )
            );
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(
                new HotelCreateViewModel
                {
                    Form =
                        new HotelCreateDto
                        {
                            Address =
                                new AddressDto()
                        }
                }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            HotelCreateViewModel model)
        {
            IReadOnlyList<string> savedPhotos =
                [];

            try
            {
                savedPhotos =
                    await _imageStorageService
                        .SaveImagesAsync(
                            model.Photos,
                            "hotels",
                            10
                        );

                model.Form.PhotoUrls =
                    savedPhotos.ToList();

                string ownerId =
                    GetCurrentUserId();

                await _hotelService.CreateAsync(
                    ownerId,
                    model.Form
                );

                return RedirectToAction(
                    nameof(Index)
                );
            }
            catch (ValidationException exception)
            {
                await _imageStorageService
                    .DeleteImagesAsync(
                        savedPhotos
                    );

                AddValidationErrors(
                    exception,
                    "Form"
                );
            }
            catch (ArgumentException exception)
            {
                await _imageStorageService
                    .DeleteImagesAsync(
                        savedPhotos
                    );

                ModelState.AddModelError(
                    nameof(model.Photos),
                    exception.Message
                );
            }

            return View(model);
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

            return View(
                new HotelEditViewModel
                {
                    Hotel = hotel,

                    Form =
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
                        }
                }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            HotelEditViewModel model)
        {
            string ownerId =
                GetCurrentUserId();

            IReadOnlyList<string> savedPhotos =
                [];

            try
            {
                savedPhotos =
                    await _imageStorageService
                        .SaveImagesAsync(
                            model.NewPhotos,
                            "hotels",
                            10
                        );

                model.Form.NewPhotoUrls =
                    savedPhotos.ToList();

                await _hotelService.UpdateAsync(
                    id,
                    ownerId,
                    model.Form
                );

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
                await _imageStorageService
                    .DeleteImagesAsync(
                        savedPhotos
                    );

                AddValidationErrors(
                    exception,
                    "Form"
                );
            }
            catch (ArgumentException exception)
            {
                await _imageStorageService
                    .DeleteImagesAsync(
                        savedPhotos
                    );

                ModelState.AddModelError(
                    nameof(model.NewPhotos),
                    exception.Message
                );
            }

            model.Hotel =
                await _hotelService
                    .GetOwnerHotelByIdAsync(
                        id,
                        ownerId
                    );

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(
            string id)
        {
            string ownerId =
                GetCurrentUserId();

            return View(
                await _hotelService
                    .GetOwnerHotelByIdAsync(
                        id,
                        ownerId
                    )
            );
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

            HotelReadDto hotel =
                await _hotelService
                    .GetOwnerHotelByIdAsync(
                        id,
                        ownerId
                    );

            await _hotelService.DeleteAsync(
                id,
                ownerId
            );

            await _imageStorageService
                .DeleteImagesAsync(
                    hotel.PhotoUrls
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
            ValidationException exception,
            string? prefix = null)
        {
            foreach (var error
                in exception.Errors)
            {
                string key =
                    string.IsNullOrWhiteSpace(
                        error.PropertyName)
                        ? string.Empty
                        : string.IsNullOrWhiteSpace(
                            prefix)
                            ? error.PropertyName
                            : $"{prefix}.{error.PropertyName}";

                ModelState.AddModelError(
                    key,
                    error.ErrorMessage
                );
            }
        }
    }
}