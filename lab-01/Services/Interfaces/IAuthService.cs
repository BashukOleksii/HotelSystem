using lab_01.DTOs.Auth;
using lab_01.DTOs.User;

namespace lab_01.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(
            LoginDto dto
        );

        Task<AuthResult> RegisterAsync(
            UserCreateDto dto
        );

        Task LogoutAsync();
    }
}