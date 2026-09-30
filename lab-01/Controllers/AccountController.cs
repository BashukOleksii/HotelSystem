using lab_01.DTOs.Auth;
using lab_01.DTOs.User;
using lab_01.Services.Interfaces;
using lab_01.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lab_01.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(
            IAuthService authService)
        {
            _authService = authService;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginDto dto,
            string? returnUrl = null)
        {
            AuthResult result =
                await _authService.LoginAsync(dto);

            if (!result.Succeeded)
            {
                foreach (string error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error
                    );
                }

                return View(dto);
            }


            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }


            return RedirectToAction(
                "Index",
                "Hotels"
            );
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (model.Password !=
                model.ConfirmPassword)
            {
                ModelState.AddModelError(
                    nameof(model.ConfirmPassword),
                    "Паролі не співпадають."
                );

                return View(model);
            }


            UserCreateDto dto =
                new UserCreateDto
                {
                    UserName =
                        model.UserName,

                    Email =
                        model.Email,

                    Password =
                        model.Password,

                    PhoneNumber =
                        model.PhoneNumber
                };


            AuthResult result =
                await _authService.RegisterAsync(
                    dto
                );


            if (!result.Succeeded)
            {
                foreach (string error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error
                    );
                }

                return View(model);
            }


            return RedirectToAction(
                nameof(Login)
            );
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();

            return RedirectToAction(
                "Index",
                "Hotels"
            );
        }


        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}