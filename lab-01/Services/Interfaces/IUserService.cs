using lab_01.Common.Pagination;
using lab_01.DTOs.User;

namespace lab_01.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserReadDto> GetByIdAsync(
            string id
        );

        Task<PagedResult<UserReadDto>> SearchAsync(
            UserFilterDto filter
        );

        Task<IReadOnlyList<string>> GetRolesAsync();

        Task<UserReadDto> CreateAsync(
            UserCreateDto dto
        );

        Task<UserReadDto> UpdateAsync(
            string id,
            UserUpdateDto dto
        );

        Task<UserReadDto> ChangeRoleAsync(
            string id,
            string roleName
        );

        Task DeleteAsync(
            string id
        );
    }
}