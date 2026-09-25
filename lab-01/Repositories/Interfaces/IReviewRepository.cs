using lab_01.Common.Pagination;
using lab_01.Models.Entities;
using lab_01.Repositories.Queries;

namespace lab_01.Repositories.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<Review?> GetByIdWithDetailsAsync(string id);

        Task<PagedResult<Review>> SearchAsync(
            ReviewQuery query
        );

        Task<double?> GetAverageRatingAsync(
            string hotelId
        );

        Task<bool> ExistsForUserAsync(
            string hotelId,
            string userId,
            string? excludeReviewId = null
        );
    }
}
