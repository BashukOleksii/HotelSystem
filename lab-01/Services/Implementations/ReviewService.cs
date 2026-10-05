using AutoMapper;
using FluentValidation;
using lab_01.Common.Pagination;
using lab_01.DTOs.Review;
using lab_01.Exceptions;
using lab_01.Models.Entities;
using lab_01.Repositories.Interfaces;
using lab_01.Repositories.Queries;
using lab_01.Services.Interfaces;

namespace lab_01.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;

        private readonly IValidator<ReviewCreateDto> _createValidator;
        private readonly IValidator<ReviewUpdateDto> _updateValidator;
        private readonly IValidator<ReviewQuery> _queryValidator;

        public ReviewService(
            IReviewRepository reviewRepository,
            IHotelRepository hotelRepository,
            IMapper mapper,
            IValidator<ReviewCreateDto> createValidator,
            IValidator<ReviewUpdateDto> updateValidator,
            IValidator<ReviewQuery> queryValidator)
        {
            _reviewRepository = reviewRepository;
            _hotelRepository = hotelRepository;
            _mapper = mapper;

            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _queryValidator = queryValidator;
        }

        public async Task<ReviewReadDto> GetByIdAsync(
            string id)
        {
            Review? review =
                await _reviewRepository
                    .GetByIdWithDetailsAsync(
                        id
                    );

            if (review is null)
            {
                throw new NotFoundException(
                    nameof(Review),
                    id
                );
            }

            return _mapper.Map<ReviewReadDto>(
                review
            );
        }

        public async Task<ReviewReadDto>
            GetUserReviewByIdAsync(
                string id,
                string userId)
        {
            Review? review =
                await _reviewRepository
                    .GetByIdWithDetailsAsync(
                        id
                    );

            if (review is null)
            {
                throw new NotFoundException(
                    nameof(Review),
                    id
                );
            }

            EnsureUserReview(
                review,
                userId
            );

            return _mapper.Map<ReviewReadDto>(
                review
            );
        }

        public async Task<PagedResult<ReviewReadDto>>
            SearchAsync(
                ReviewFilterDto filter)
        {
            ReviewQuery query =
                CreateQuery(
                    filter
                );

            await _queryValidator
                .ValidateAndThrowAsync(
                    query
                );

            PagedResult<Review> result =
                await _reviewRepository
                    .SearchAsync(
                        query
                    );

            return MapPagedResult(
                result
            );
        }

        public async Task<PagedResult<ReviewReadDto>>
            SearchUserReviewsAsync(
                string userId,
                ReviewFilterDto filter)
        {
            ReviewQuery query =
                CreateQuery(
                    filter,
                    userId
                );

            await _queryValidator
                .ValidateAndThrowAsync(
                    query
                );

            PagedResult<Review> result =
                await _reviewRepository
                    .SearchAsync(
                        query
                    );

            return MapPagedResult(
                result
            );
        }

        public async Task<double?> GetAverageRatingAsync(
            string hotelId)
        {
            await GetHotelAsync(
                hotelId
            );

            return await _reviewRepository
                .GetAverageRatingAsync(
                    hotelId
                );
        }

        public async Task<ReviewReadDto> CreateAsync(
            string userId,
            ReviewCreateDto dto)
        {
            await _createValidator
                .ValidateAndThrowAsync(
                    dto
                );

            await GetHotelAsync(
                dto.HotelId
            );

            bool reviewExists =
                await _reviewRepository
                    .ExistsForUserAsync(
                        dto.HotelId,
                        userId
                    );

            if (reviewExists)
            {
                throw new ConflictException(
                    "Ви вже залишили відгук для цього готелю."
                );
            }

            Review review =
                _mapper.Map<Review>(
                    dto
                );

            review.UserId = userId;

            await _reviewRepository.AddAsync(
                review
            );

            await _reviewRepository
                .SaveChangesAsync();

            return await GetDetailedReviewAsync(
                review.Id
            );
        }

        public async Task<ReviewReadDto> UpdateAsync(
            string id,
            string userId,
            ReviewUpdateDto dto)
        {
            await _updateValidator
                .ValidateAndThrowAsync(
                    dto
                );

            Review? review =
                await _reviewRepository.GetByIdAsync(
                    id
                );

            if (review is null)
            {
                throw new NotFoundException(
                    nameof(Review),
                    id
                );
            }

            EnsureUserReview(
                review,
                userId
            );

            _mapper.Map(
                dto,
                review
            );

            _reviewRepository.Update(
                review
            );

            await _reviewRepository
                .SaveChangesAsync();

            return await GetDetailedReviewAsync(
                review.Id
            );
        }

        public async Task DeleteAsync(
            string id,
            string userId)
        {
            Review? review =
                await _reviewRepository.GetByIdAsync(
                    id
                );

            if (review is null)
            {
                throw new NotFoundException(
                    nameof(Review),
                    id
                );
            }

            EnsureUserReview(
                review,
                userId
            );

            _reviewRepository.Delete(
                review
            );

            await _reviewRepository
                .SaveChangesAsync();
        }

        public async Task DeleteAsAdminAsync(
            string id)
        {
            Review? review =
                await _reviewRepository.GetByIdAsync(
                    id
                );

            if (review is null)
            {
                throw new NotFoundException(
                    nameof(Review),
                    id
                );
            }

            _reviewRepository.Delete(
                review
            );

            await _reviewRepository
                .SaveChangesAsync();
        }

        private async Task<ReviewReadDto>
            GetDetailedReviewAsync(
                string id)
        {
            Review? review =
                await _reviewRepository
                    .GetByIdWithDetailsAsync(
                        id
                    );

            if (review is null)
            {
                throw new NotFoundException(
                    nameof(Review),
                    id
                );
            }

            return _mapper.Map<ReviewReadDto>(
                review
            );
        }

        private static ReviewQuery CreateQuery(
            ReviewFilterDto filter,
            string? userId = null)
        {
            return new ReviewQuery
            {
                Search = filter.Search,
                HotelId = filter.HotelId,
                UserId = userId,
                MinRating = filter.MinRating,
                MaxRating = filter.MaxRating,
                SortBy = filter.SortBy,
                Descending = filter.Descending,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        private PagedResult<ReviewReadDto>
            MapPagedResult(
                PagedResult<Review> result)
        {
            return new PagedResult<ReviewReadDto>
            {
                Items =
                    _mapper.Map<
                        IReadOnlyList<ReviewReadDto>
                    >(result.Items),

                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        private async Task<Hotel> GetHotelAsync(
            string hotelId)
        {
            Hotel? hotel =
                await _hotelRepository.GetByIdAsync(
                    hotelId
                );

            if (hotel is null)
            {
                throw new NotFoundException(
                    nameof(Hotel),
                    hotelId
                );
            }

            return hotel;
        }

        private static void EnsureUserReview(
            Review review,
            string userId)
        {
            if (review.UserId != userId)
            {
                throw new ForbiddenOperationException(
                    "Ви не можете керувати чужим відгуком."
                );
            }
        }
    }
}