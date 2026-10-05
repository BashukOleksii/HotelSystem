using FluentValidation;
using lab_01.DTOs.User;
using lab_01.Exceptions;
using lab_01.Services.Interfaces;
using lab_01.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lab_01.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private static readonly string[] ManagedRoles =
        [
            "User",
            "Owner"
        ];

        private readonly IUserService _userService;

        public UsersController(
            IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] UserFilterDto filter)
        {
            AdminUserIndexViewModel model =
                new AdminUserIndexViewModel
                {
                    Filter = filter
                };

            try
            {
                model.Users =
                    await _userService.SearchAsync(
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
            UserReadDto user =
                await _userService.GetByIdAsync(
                    id
                );

            return View(user);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(
                new AdminUserCreateViewModel
                {
                    Roles = ManagedRoles,
                    SelectedRole = "User"
                }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            AdminUserCreateViewModel model)
        {
            model.Roles =
                ManagedRoles;

            if (!IsManagedRole(
                model.SelectedRole))
            {
                ModelState.AddModelError(
                    nameof(model.SelectedRole),
                    "Можна вибрати тільки роль User або Owner."
                );
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                UserReadDto user =
                    await _userService.CreateAsync(
                        model.Form
                    );

                if (model.SelectedRole != "User")
                {
                    user =
                        await _userService
                            .ChangeRoleAsync(
                                user.Id,
                                model.SelectedRole
                            );
                }

                TempData["Success"] =
                    "Користувача створено.";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = user.Id
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
            catch (IdentityOperationException exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    exception.Message
                );
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(
            string id)
        {
            UserReadDto user =
                await GetManageableUserAsync(
                    id
                );

            return View(
                BuildEditModel(
                    user
                )
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            AdminUserEditViewModel model)
        {
            UserReadDto currentUser =
                await GetManageableUserAsync(
                    id
                );

            model.User =
                currentUser;

            model.Roles =
                ManagedRoles;

            if (!IsManagedRole(
                model.SelectedRole))
            {
                ModelState.AddModelError(
                    nameof(model.SelectedRole),
                    "Можна вибрати тільки роль User або Owner."
                );
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _userService.UpdateAsync(
                    id,
                    model.Form
                );

                string? currentRole =
                    currentUser.Roles
                        .FirstOrDefault(
                            IsManagedRole
                        );

                if (currentRole !=
                    model.SelectedRole)
                {
                    await _userService
                        .ChangeRoleAsync(
                            id,
                            model.SelectedRole
                        );
                }

                TempData["Success"] =
                    "Дані користувача оновлено.";

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
            catch (IdentityOperationException exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    exception.Message
                );
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(
            string id)
        {
            UserReadDto user =
                await GetManageableUserAsync(
                    id
                );

            return View(user);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            DeleteConfirmed(
                string id)
        {
            await GetManageableUserAsync(
                id
            );

            try
            {
                await _userService.DeleteAsync(
                    id
                );

                TempData["Success"] =
                    "Користувача видалено.";

                return RedirectToAction(
                    nameof(Index)
                );
            }
            catch (IdentityOperationException exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    exception.Message
                );

                UserReadDto user =
                    await _userService.GetByIdAsync(
                        id
                    );

                return View(
                    "Delete",
                    user
                );
            }
        }

        private async Task<UserReadDto>
            GetManageableUserAsync(
                string id)
        {
            UserReadDto user =
                await _userService.GetByIdAsync(
                    id
                );

            if (user.Roles.Contains(
                "Admin"
            ))
            {
                throw new ForbiddenOperationException(
                    "Адміністраторів не можна змінювати або видаляти через цю сторінку."
                );
            }

            return user;
        }

        private static AdminUserEditViewModel
            BuildEditModel(
                UserReadDto user)
        {
            return new AdminUserEditViewModel
            {
                User = user,

                Form =
                    new UserUpdateDto
                    {
                        UserName =
                            user.UserName,

                        Email =
                            user.Email,

                        PhoneNumber =
                            user.PhoneNumber
                    },

                SelectedRole =
                    user.Roles
                        .FirstOrDefault(
                            IsManagedRole
                        )
                    ?? "User",

                Roles =
                    ManagedRoles
            };
        }

        private static bool IsManagedRole(
            string role)
        {
            return ManagedRoles.Contains(
                role
            );
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