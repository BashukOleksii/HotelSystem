using FluentValidation;
using lab_01.DTOs.Review;
using lab_01.Services.Interfaces;
using lab_01.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lab_01.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IImageStorageService _imageStorageService;

        public ReviewsController(
            IReviewService reviewService,
            IImageStorageService imageStorageService)
        {
            _reviewService =
                reviewService;

            _imageStorageService =
                imageStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] ReviewFilterDto filter)
        {
            AdminReviewIndexViewModel model =
                new AdminReviewIndexViewModel
                {
                    Filter = filter
                };

            try
            {
                model.Reviews =
                    await _reviewService.SearchAsync(
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
            ReviewReadDto review =
                await _reviewService.GetByIdAsync(
                    id
                );

            return View(review);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(
            string id)
        {
            ReviewReadDto review =
                await _reviewService.GetByIdAsync(
                    id
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
            ReviewReadDto review =
                await _reviewService.GetByIdAsync(
                    id
                );

            await _reviewService
                .DeleteAsAdminAsync(
                    id
                );

            await _imageStorageService
                .DeleteImagesAsync(
                    review.PhotoUrls
                );

            TempData["Success"] =
                "Відгук видалено адміністратором.";

            return RedirectToAction(
                nameof(Index)
            );
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