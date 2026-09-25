using lab_01.Common.Pagination;
using lab_01.Models.Entities;
using lab_01.Repositories.Queries;

namespace lab_01.Repositories.Interfaces
{
    public interface IRoomRepository : IRepository<Room>
    {
        Task<Room?> GetByIdWithDetailsAsync(string id);

        Task<PagedResult<Room>> SearchAsync(RoomQuery query);

        Task<bool> ExistsByNumberAsync(
            string hotelId,
            string roomNumber,
            string? excludeRoomId = null
        );

        Task<bool> IsAvailableAsync(
            string roomId,
            DateTime checkIn,
            DateTime checkOut
        );
    }
}
