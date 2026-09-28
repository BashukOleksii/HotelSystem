using AutoMapper;
using FluentValidation;
using lab_01.Common.Pagination;
using lab_01.DTOs.Room;
using lab_01.Exceptions;
using lab_01.Models.Entities;
using lab_01.Repositories.Interfaces;
using lab_01.Repositories.Queries;
using lab_01.Services.Interfaces;

namespace lab_01.Services.Implementations
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IHotelRepository _hotelRepository;

        private readonly IMapper _mapper;

        private readonly IValidator<RoomCreateDto> _createValidator;
        private readonly IValidator<RoomUpdateDto> _updateValidator;
        private readonly IValidator<RoomQuery> _queryValidator;

        public RoomService(
            IRoomRepository roomRepository,
            IHotelRepository hotelRepository,
            IMapper mapper,
            IValidator<RoomCreateDto> createValidator,
            IValidator<RoomUpdateDto> updateValidator,
            IValidator<RoomQuery> queryValidator)
        {
            _roomRepository = roomRepository;
            _hotelRepository = hotelRepository;
            _mapper = mapper;

            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _queryValidator = queryValidator;
        }

        public async Task<RoomReadDto> GetByIdAsync(
            string id)
        {
            Room? room =
                await _roomRepository.GetByIdAsync(
                    id
                );

            if (room is null)
            {
                throw new NotFoundException(
                    nameof(Room),
                    id
                );
            }

            return _mapper.Map<RoomReadDto>(
                room
            );
        }

        public async Task<PagedResult<RoomReadDto>>
            SearchAsync(
                RoomFilterDto filter)
        {
            RoomQuery query =
                CreateQuery(filter);

            await _queryValidator
                .ValidateAndThrowAsync(
                    query
                );

            PagedResult<Room> result =
                await _roomRepository.SearchAsync(
                    query
                );

            return MapPagedResult(result);
        }

        public async Task<PagedResult<RoomReadDto>>
            SearchOwnerHotelRoomsAsync(
                string ownerId,
                string hotelId,
                RoomFilterDto filter)
        {
            await GetOwnedHotelAsync(
                hotelId,
                ownerId
            );

            RoomQuery query =
                CreateQuery(
                    filter,
                    hotelId
                );

            await _queryValidator
                .ValidateAndThrowAsync(
                    query
                );

            PagedResult<Room> result =
                await _roomRepository.SearchAsync(
                    query
                );

            return MapPagedResult(result);
        }

        public async Task<RoomReadDto> CreateAsync(
            string ownerId,
            RoomCreateDto dto)
        {
            await _createValidator
                .ValidateAndThrowAsync(
                    dto
                );

            await GetOwnedHotelAsync(
                dto.HotelId,
                ownerId
            );

            string roomNumber =
                dto.RoomNumber.Trim();

            bool roomExists =
                await _roomRepository
                    .ExistsByNumberAsync(
                        dto.HotelId,
                        roomNumber
                    );

            if (roomExists)
            {
                throw new ConflictException(
                    $"Кімната з номером '{roomNumber}' уже існує в цьому готелі."
                );
            }

            Room room =
                _mapper.Map<Room>(
                    dto
                );

            room.RoomNumber =
                roomNumber;

            await _roomRepository.AddAsync(
                room
            );

            await _roomRepository
                .SaveChangesAsync();

            return _mapper.Map<RoomReadDto>(
                room
            );
        }

        public async Task<RoomReadDto> UpdateAsync(
            string id,
            string ownerId,
            RoomUpdateDto dto)
        {
            await _updateValidator
                .ValidateAndThrowAsync(
                    dto
                );

            Room? room =
                await _roomRepository.GetByIdAsync(
                    id
                );

            if (room is null)
            {
                throw new NotFoundException(
                    nameof(Room),
                    id
                );
            }

            await GetOwnedHotelAsync(
                room.HotelId,
                ownerId
            );

            if (dto.RoomNumber is not null)
            {
                string roomNumber =
                    dto.RoomNumber.Trim();

                bool roomExists =
                    await _roomRepository
                        .ExistsByNumberAsync(
                            room.HotelId,
                            roomNumber,
                            room.Id
                        );

                if (roomExists)
                {
                    throw new ConflictException(
                        $"Кімната з номером '{roomNumber}' уже існує в цьому готелі."
                    );
                }

                dto.RoomNumber =
                    roomNumber;
            }

            _mapper.Map(
                dto,
                room
            );

            _roomRepository.Update(
                room
            );

            await _roomRepository
                .SaveChangesAsync();

            return _mapper.Map<RoomReadDto>(
                room
            );
        }

        public async Task DeleteAsync(
            string id,
            string ownerId)
        {
            Room? room =
                await _roomRepository.GetByIdAsync(
                    id
                );

            if (room is null)
            {
                throw new NotFoundException(
                    nameof(Room),
                    id
                );
            }

            await GetOwnedHotelAsync(
                room.HotelId,
                ownerId
            );

            _roomRepository.Delete(
                room
            );

            await _roomRepository
                .SaveChangesAsync();
        }

        private async Task<Hotel> GetOwnedHotelAsync(
            string hotelId,
            string ownerId)
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

            if (hotel.OwnerId != ownerId)
            {
                throw new ForbiddenOperationException(
                    "Ви не можете керувати кімнатами чужого готелю."
                );
            }

            return hotel;
        }

        private static RoomQuery CreateQuery(
            RoomFilterDto filter,
            string? forcedHotelId = null)
        {
            return new RoomQuery
            {
                Search = filter.Search,

                HotelId =
                    forcedHotelId ??
                    filter.HotelId,

                Type = filter.Type,

                MinPrice = filter.MinPrice,
                MaxPrice = filter.MaxPrice,

                MinCapacity = filter.MinCapacity,

                IsAvailable = filter.IsAvailable,

                CheckIn = filter.CheckIn,
                CheckOut = filter.CheckOut,

                SortBy = filter.SortBy,
                Descending = filter.Descending,

                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        private PagedResult<RoomReadDto> MapPagedResult(
            PagedResult<Room> result)
        {
            return new PagedResult<RoomReadDto>
            {
                Items =
                    _mapper.Map<
                        IReadOnlyList<RoomReadDto>
                    >(result.Items),

                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}