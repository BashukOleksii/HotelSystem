using AutoMapper;
using FluentValidation;
using lab_01.Common.Pagination;
using lab_01.DTOs.Hotel;
using lab_01.Exceptions;
using lab_01.Models.Entities;
using lab_01.Repositories.Interfaces;
using lab_01.Repositories.Queries;
using lab_01.Services.Interfaces;

namespace lab_01.Services.Implementations
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;

        private readonly IValidator<HotelCreateDto> _createValidator;
        private readonly IValidator<HotelUpdateDto> _updateValidator;
        private readonly IValidator<HotelQuery> _queryValidator;

        public HotelService(
            IHotelRepository hotelRepository,
            IMapper mapper,
            IValidator<HotelCreateDto> createValidator,
            IValidator<HotelUpdateDto> updateValidator,
            IValidator<HotelQuery> queryValidator)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;

            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _queryValidator = queryValidator;
        }

        public async Task<HotelReadDto> GetByIdAsync(
            string id)
        {
            Hotel? hotel =
                await _hotelRepository.GetByIdAsync(id);

            if (hotel is null)
            {
                throw new NotFoundException(
                    nameof(Hotel),
                    id
                );
            }

            return _mapper.Map<HotelReadDto>(hotel);
        }

        private static HotelQuery CreateQuery(
            HotelFilterDto filter,
            string? ownerId = null)
        {
            return new HotelQuery
            {
                Search = filter.Search,
                City = filter.City,
                SortBy = filter.SortBy,
                Descending = filter.Descending,
                Page = filter.Page,
                PageSize = filter.PageSize,
                OwnerId = ownerId
            };
        }

        public async Task<HotelReadDto> GetOwnerHotelByIdAsync(
            string id,
            string ownerId)
        {
            Hotel? hotel =
                await _hotelRepository.GetByIdAsync(
                    id
                );

            if (hotel is null)
            {
                throw new NotFoundException(
                    nameof(Hotel),
                    id
                );
            }

            EnsureOwner(
                hotel,
                ownerId
            );

            return _mapper.Map<HotelReadDto>(
                hotel
            );
        }

        public async Task<PagedResult<HotelReadDto>>
        SearchAsync(
            HotelFilterDto filter)
        {
            HotelQuery query =
                CreateQuery(filter);

            await _queryValidator
                .ValidateAndThrowAsync(query);

            PagedResult<Hotel> result =
                await _hotelRepository.SearchAsync(
                    query
                );

            return MapPagedResult(result);
        }

        public async Task<PagedResult<HotelReadDto>>
        SearchOwnerHotelsAsync(
            string ownerId,
            HotelFilterDto filter)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
            {
                throw new ArgumentException(
                    "OwnerId не може бути порожнім.",
                    nameof(ownerId)
                );
            }

            HotelQuery query =
                CreateQuery(
                    filter,
                    ownerId
                );

            await _queryValidator
                .ValidateAndThrowAsync(query);

            PagedResult<Hotel> result =
                await _hotelRepository.SearchAsync(
                    query
                );

            return MapPagedResult(result);
        }

        public async Task<HotelReadDto> CreateAsync(
            string ownerId,
            HotelCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
            {
                throw new ArgumentException(
                    "OwnerId не може бути порожнім.",
                    nameof(ownerId)
                );
            }

            await _createValidator
                .ValidateAndThrowAsync(dto);

            Hotel hotel =
                _mapper.Map<Hotel>(dto);

            hotel.OwnerId = ownerId;

            await _hotelRepository.AddAsync(
                hotel
            );

            await _hotelRepository
                .SaveChangesAsync();

            return _mapper.Map<HotelReadDto>(
                hotel
            );
        }

        private PagedResult<HotelReadDto> MapPagedResult(
            PagedResult<Hotel> result)
        {
            return new PagedResult<HotelReadDto>
            {
                Items = _mapper.Map<IReadOnlyList<HotelReadDto>>(
                    result.Items
                ),

                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<HotelReadDto> UpdateAsync(
            string id,
            string ownerId,
            HotelUpdateDto dto)
        {
            await _updateValidator
                .ValidateAndThrowAsync(dto);

            Hotel? hotel =
                await _hotelRepository.GetByIdAsync(
                    id
                );

            if (hotel is null)
            {
                throw new NotFoundException(
                    nameof(Hotel),
                    id
                );
            }

            EnsureOwner(
                hotel,
                ownerId
            );

            _mapper.Map(
                dto,
                hotel
            );

            _hotelRepository.Update(
                hotel
            );

            await _hotelRepository
                .SaveChangesAsync();

            return _mapper.Map<HotelReadDto>(
                hotel
            );
        }

        public async Task DeleteAsync(
            string id,
            string ownerId)
        {
            Hotel? hotel =
                await _hotelRepository.GetByIdAsync(
                    id
                );

            if (hotel is null)
            {
                throw new NotFoundException(
                    nameof(Hotel),
                    id
                );
            }

            EnsureOwner(
                hotel,
                ownerId
            );

            _hotelRepository.Delete(
                hotel
            );

            await _hotelRepository
                .SaveChangesAsync();
        }

        private static void EnsureOwner(
            Hotel hotel,
            string ownerId)
        {
            if (hotel.OwnerId != ownerId)
            {
                throw new ForbiddenOperationException(
                    "Ви не можете керувати чужим готелем."
                );
            }
        }

    }
}