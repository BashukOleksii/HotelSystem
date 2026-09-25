using lab_01.Common.Pagination;
using lab_01.Models.Entities;
using lab_01.Repositories.Queries;

namespace lab_01.Repositories.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<Booking?> GetByIdWithDetailsAsync(string id);

        Task<PagedResult<Booking>> SearchAsync(
            BookingQuery query
        );

        Task<bool> HasConflictAsync(
            string roomId,
            DateTime checkIn,
            DateTime checkOut,
            string? excludeBookingId = null
        );
    }
}
