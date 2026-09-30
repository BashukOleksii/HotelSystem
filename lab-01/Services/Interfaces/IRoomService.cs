using lab_01.Common.Pagination;
using lab_01.DTOs.Room;

namespace lab_01.Services.Interfaces
{
    public interface IRoomService
    {
        Task<RoomReadDto> GetByIdAsync(
            string id
        );

        Task<RoomReadDto> GetOwnerRoomByIdAsync(
            string id,
            string ownerId
        );

        Task<PagedResult<RoomReadDto>> SearchAsync(
            RoomFilterDto filter
        );

        Task<PagedResult<RoomReadDto>> SearchOwnerHotelRoomsAsync(
            string ownerId,
            string hotelId,
            RoomFilterDto filter
        );

        Task<RoomReadDto> CreateAsync(
            string ownerId,
            RoomCreateDto dto
        );

        Task<RoomReadDto> UpdateAsync(
            string id,
            string ownerId,
            RoomUpdateDto dto
        );

        Task DeleteAsync(
            string id,
            string ownerId
        );
    }
}