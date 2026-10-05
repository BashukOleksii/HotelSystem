using System.Security.Claims;
using FluentValidation;
using lab_01.DTOs.Hotel;
using lab_01.DTOs.Review;
using lab_01.Exceptions;
using lab_01.Services.Interfaces;
using lab_01.ViewModels.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lab_01.Controllers
{
    [Authorize(Roles = "User")]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IHotelService _hotelService;
        private readonly IImageStorageService _imageStorageService;

        public ReviewsController(
            IReviewService reviewService,
            IHotelService hotelService,
            IImageStorageService imageStorageService)
        {
            _reviewService =
                reviewService;

            _hotelService =
                hotelService;

            _imageStorageService =
                imageStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] ReviewFilterDto filter)
        {
            string userId =
                GetCurrentUserId();

            UserReviewIndexViewModel model =
                new UserReviewIndexViewModel
                {
                    Filter = filter
                };

            try
            {
                model.Reviews =
                    await _reviewService
                        .SearchUserReviewsAsync(
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

            return View(
                await _reviewService
                    .GetUserReviewByIdAsync(
                        id,
                        userId
                    )
            );
        }

        [HttpGet]
        public async Task<IActionResult> Create(
            string hotelId)
        {
            HotelReadDto hotel =
                await _hotelService.GetByIdAsync(
                    hotelId
                );

            return View(
                new ReviewCreateViewModel
                {
                    Hotel = hotel,

                    Form =
                        new ReviewCreateDto
                        {
                            HotelId =
                                hotelId,

                            Rating =
                                5
                        }
                }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ReviewCreateViewModel model)
        {
            IReadOnlyList<string> savedPhotos =
                [];

            try
            {
                savedPhotos =
                    await _imageStorageService
                        .SaveImagesAsync(
                            model.Photos,
                            "reviews",
                            5
                        );

                model.Form.PhotoUrls =
                    savedPhotos.ToList();

                string userId =
                    GetCurrentUserId();

                ReviewReadDto review =
                    await _reviewService.CreateAsync(
                        userId,
                        model.Form
                    );

                TempData["Success"] =
                    "Відгук успішно додано.";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = review.Id
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
            catch (ConflictException exception)
            {
                await _imageStorageService
                    .DeleteImagesAsync(
                        savedPhotos
                    );

                ModelState.AddModelError(
                    string.Empty,
                    exception.Message
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

            model.Hotel =
                await _hotelService.GetByIdAsync(
                    model.Form.HotelId
                );

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(
            string id)
        {
            string userId =
                GetCurrentUserId();

            ReviewReadDto review =
                await _reviewService
                    .GetUserReviewByIdAsync(
                        id,
                        userId
                    );

            return View(
                new ReviewEditViewModel
                {
                    Review = review,

                    Form =
                        new ReviewUpdateDto
                        {
                            Rating =
                                review.Rating,

                            Comment =
                                review.Comment
                        }
                }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            ReviewEditViewModel model)
        {
            string userId =
                GetCurrentUserId();

            IReadOnlyList<string> savedPhotos =
                [];

            try
            {
                savedPhotos =
                    await _imageStorageService
                        .SaveImagesAsync(
                            model.NewPhotos,
                            "reviews",
                            5
                        );

                model.Form.NewPhotoUrls =
                    savedPhotos.ToList();

                await _reviewService.UpdateAsync(
                    id,
                    userId,
                    model.Form
                );

                TempData["Success"] =
                    "Відгук успішно оновлено.";

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

            model.Review =
                await _reviewService
                    .GetUserReviewByIdAsync(
                        id,
                        userId
                    );

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(
            string id)
        {
            string userId =
                GetCurrentUserId();

            return View(
                await _reviewService
                    .GetUserReviewByIdAsync(
                        id,
                        userId
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
            string userId =
                GetCurrentUserId();

            ReviewReadDto review =
                await _reviewService
                    .GetUserReviewByIdAsync(
                        id,
                        userId
                    );

            await _reviewService.DeleteAsync(
                id,
                userId
            );

            await _imageStorageService
                .DeleteImagesAsync(
                    review.PhotoUrls
                );

            TempData["Success"] =
                "Відгук видалено.";

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