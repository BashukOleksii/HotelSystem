using lab_01.Common.Pagination;
using lab_01.DTOs.Review;

namespace lab_01.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewReadDto> GetByIdAsync(
            string id
        );

        Task<PagedResult<ReviewReadDto>> SearchAsync(
            ReviewFilterDto filter
        );

        Task<PagedResult<ReviewReadDto>> SearchUserReviewsAsync(
            string userId,
            ReviewFilterDto filter
        );

        Task<double?> GetAverageRatingAsync(
            string hotelId
        );

        Task<ReviewReadDto> CreateAsync(
            string userId,
            ReviewCreateDto dto
        );

        Task<ReviewReadDto> UpdateAsync(
            string id,
            string userId,
            ReviewUpdateDto dto
        );

        Task DeleteAsync(
            string id,
            string userId
        );
    }
}