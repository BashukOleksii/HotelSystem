using lab_01.Common.Pagination;
using lab_01.DTOs.Hotel;

namespace lab_01.Services.Interfaces
{
    public interface IHotelService
    {
        Task<HotelReadDto> GetByIdAsync(
            string id
        );

        Task<HotelReadDto> GetOwnerHotelByIdAsync(
            string id,
            string ownerId
        );

        Task<PagedResult<HotelReadDto>> SearchAsync(
            HotelFilterDto filter
        );

        Task<PagedResult<HotelReadDto>> SearchOwnerHotelsAsync(
            string ownerId,
            HotelFilterDto filter
        );

        Task<HotelReadDto> CreateAsync(
            string ownerId,
            HotelCreateDto dto
        );

        Task<HotelReadDto> UpdateAsync(
            string id,
            string ownerId,
            HotelUpdateDto dto
        );

        Task DeleteAsync(
            string id,
            string ownerId
        );
    }
}