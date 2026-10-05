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

        public ReviewsController(
            IReviewService reviewService,
            IHotelService hotelService)
        {
            _reviewService =
                reviewService;

            _hotelService =
                hotelService;
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

            ReviewReadDto review =
                await _reviewService
                    .GetUserReviewByIdAsync(
                        id,
                        userId
                    );

            return View(review);
        }

        [HttpGet]
        public async Task<IActionResult> Create(
            string hotelId)
        {
            HotelReadDto hotel =
                await _hotelService.GetByIdAsync(
                    hotelId
                );

            ReviewCreateViewModel model =
                new ReviewCreateViewModel
                {
                    Hotel = hotel,

                    Form =
                        new ReviewCreateDto
                        {
                            HotelId = hotelId,
                            Rating = 5
                        }
                };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ReviewCreateViewModel model)
        {
            try
            {
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

            ReviewEditViewModel model =
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
                };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            ReviewEditViewModel model)
        {
            string userId =
                GetCurrentUserId();

            try
            {
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
                AddValidationErrors(
                    exception,
                    "Form"
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

            ReviewReadDto review =
                await _reviewService
                    .GetUserReviewByIdAsync(
                        id,
                        userId
                    );

            return View(review);
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

            await _reviewService.DeleteAsync(
                id,
                userId
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
                    key =
                        error.PropertyName;
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