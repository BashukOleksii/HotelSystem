using lab_01.Common.Pagination;
using lab_01.DTOs.Booking;

namespace lab_01.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingReadDto> GetUserBookingByIdAsync(
            string id,
            string userId
        );

        Task<BookingReadDto> GetOwnerBookingByIdAsync(
            string id,
            string ownerId
        );

        Task<PagedResult<BookingReadDto>> SearchUserBookingsAsync(
            string userId,
            BookingFilterDto filter
        );

        Task<PagedResult<BookingReadDto>> SearchOwnerBookingsAsync(
            string ownerId,
            BookingFilterDto filter
        );

        Task<BookingReadDto> CreateAsync(
            string userId,
            BookingCreateDto dto
        );

        Task<BookingReadDto> UpdateAsync(
            string id,
            string userId,
            BookingUpdateDto dto
        );

        Task DeleteAsync(
            string id,
            string userId
        );
    }
}