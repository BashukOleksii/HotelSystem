using FluentValidation;
using lab_01.DTOs.Auth;
using lab_01.DTOs.User;
using lab_01.Models.Entities;
using lab_01.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace lab_01.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private const string DefaultRole = "User";

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<UserCreateDto> _registerValidator;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            IValidator<LoginDto> loginValidator,
            IValidator<UserCreateDto> registerValidator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;

            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
        }

        public async Task<AuthResult> LoginAsync(
            LoginDto dto)
        {
            var validationResult =
                await _loginValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return new AuthResult
                {
                    Succeeded = false,

                    Errors = validationResult.Errors
                        .Select(error =>
                            error.ErrorMessage
                        )
                        .ToList()
                };
            }


            User? user =
                await _userManager.FindByEmailAsync(
                    dto.Email.Trim()
                );

            if (user is null)
            {
                return AuthResult.Failure(
                    "Невірний Email або пароль."
                );
            }


            SignInResult result =
                await _signInManager.PasswordSignInAsync(
                    user,
                    dto.Password,
                    dto.RememberMe,
                    lockoutOnFailure: false
                );


            if (!result.Succeeded)
            {
                return AuthResult.Failure(
                    "Невірний Email або пароль."
                );
            }


            return AuthResult.Success();
        }


        public async Task<AuthResult> RegisterAsync(
            UserCreateDto dto)
        {
            var validationResult =
                await _registerValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return new AuthResult
                {
                    Succeeded = false,

                    Errors = validationResult.Errors
                        .Select(error =>
                            error.ErrorMessage
                        )
                        .ToList()
                };
            }


            User user = new User
            {
                UserName =
                    dto.UserName.Trim(),

                Email =
                    dto.Email.Trim(),

                PhoneNumber =
                    dto.PhoneNumber?.Trim(),

                EmailConfirmed = true
            };


            IdentityResult createResult =
                await _userManager.CreateAsync(
                    user,
                    dto.Password
                );


            if (!createResult.Succeeded)
            {
                return new AuthResult
                {
                    Succeeded = false,

                    Errors = createResult.Errors
                        .Select(error =>
                            error.Description
                        )
                        .ToList()
                };
            }


            bool roleExists =
                await _roleManager.RoleExistsAsync(
                    DefaultRole
                );

            if (!roleExists)
            {
                await _userManager.DeleteAsync(
                    user
                );

                return AuthResult.Failure(
                    $"Роль '{DefaultRole}' не знайдена."
                );
            }


            IdentityResult roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    DefaultRole
                );


            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(
                    user
                );

                return new AuthResult
                {
                    Succeeded = false,

                    Errors = roleResult.Errors
                        .Select(error =>
                            error.Description
                        )
                        .ToList()
                };
            }


            return AuthResult.Success();
        }


        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}