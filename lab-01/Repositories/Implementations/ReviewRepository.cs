using lab_01.Common.Pagination;
using lab_01.Data;
using lab_01.Models.Entities;
using lab_01.Repositories.Interfaces;
using lab_01.Repositories.Queries;
using Microsoft.EntityFrameworkCore;

namespace lab_01.Repositories.Implementations
{
    public class ReviewRepository
        : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(
            AppDbContext context)
            : base(context)
        {
        }

        public async Task<Review?> GetByIdWithDetailsAsync(
            string id)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Include(review => review.User)
                .Include(review => review.Hotel)
                .FirstOrDefaultAsync(
                    review => review.Id == id
                );
        }

        public async Task<PagedResult<Review>> SearchAsync(
            ReviewQuery query)
        {
            IQueryable<Review> reviews =
                _context.Reviews
                    .AsNoTracking()
                    .Include(review => review.User)
                    .Include(review => review.Hotel);

            if (!string.IsNullOrWhiteSpace(
                query.Search))
            {
                string search =
                    query.Search.Trim();

                reviews =
                    reviews.Where(review =>
                        review.Comment != null &&
                        EF.Functions.Like(
                            review.Comment,
                            $"%{search}%"
                        )
                    );
            }

            if (!string.IsNullOrWhiteSpace(
                query.HotelId))
            {
                reviews =
                    reviews.Where(
                        review =>
                            review.HotelId ==
                            query.HotelId
                    );
            }

            if (!string.IsNullOrWhiteSpace(
                query.UserId))
            {
                reviews =
                    reviews.Where(
                        review =>
                            review.UserId ==
                            query.UserId
                    );
            }

            if (query.MinRating.HasValue)
            {
                reviews =
                    reviews.Where(
                        review =>
                            review.Rating >=
                            query.MinRating.Value
                    );
            }

            if (query.MaxRating.HasValue)
            {
                reviews =
                    reviews.Where(
                        review =>
                            review.Rating <=
                            query.MaxRating.Value
                    );
            }

            int totalCount =
                await reviews.CountAsync();

            reviews =
                ApplySorting(
                    reviews,
                    query
                );

            List<Review> items =
                await reviews
                    .Skip(
                        (query.Page - 1) *
                        query.PageSize
                    )
                    .Take(
                        query.PageSize
                    )
                    .ToListAsync();

            return new PagedResult<Review>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<double?> GetAverageRatingAsync(
            string hotelId)
        {
            IQueryable<int> ratings =
                _context.Reviews
                    .Where(
                        review =>
                            review.HotelId ==
                            hotelId
                    )
                    .Select(
                        review =>
                            review.Rating
                    );

            if (!await ratings.AnyAsync())
            {
                return null;
            }

            return await ratings.AverageAsync();
        }

        public async Task<bool> ExistsForUserAsync(
            string hotelId,
            string userId,
            string? excludeReviewId = null)
        {
            return await _context.Reviews.AnyAsync(
                review =>
                    review.HotelId == hotelId &&
                    review.UserId == userId &&
                    (
                        excludeReviewId == null ||
                        review.Id != excludeReviewId
                    )
            );
        }

        private static IQueryable<Review>
            ApplySorting(
                IQueryable<Review> reviews,
                ReviewQuery query)
        {
            return query.SortBy?.ToLower()
                switch
            {
                "rating" =>
                    query.Descending
                        ? reviews.OrderByDescending(
                            review =>
                                review.Rating
                        )
                        : reviews.OrderBy(
                            review =>
                                review.Rating
                        ),

                "createdat" =>
                    query.Descending
                        ? reviews.OrderByDescending(
                            review =>
                                review.CreatedAt
                        )
                        : reviews.OrderBy(
                            review =>
                                review.CreatedAt
                        ),

                _ =>
                    reviews.OrderByDescending(
                        review =>
                            review.CreatedAt
                    )
            };
        }
    }
}