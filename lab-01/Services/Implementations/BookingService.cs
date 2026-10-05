using AutoMapper;
using FluentValidation;
using lab_01.Common.Pagination;
using lab_01.DTOs.Booking;
using lab_01.Exceptions;
using lab_01.Models.Entities;
using lab_01.Repositories.Interfaces;
using lab_01.Repositories.Queries;
using lab_01.Services.Interfaces;

namespace lab_01.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;

        private readonly IValidator<BookingCreateDto> _createValidator;
        private readonly IValidator<BookingUpdateDto> _updateValidator;
        private readonly IValidator<BookingQuery> _queryValidator;

        public BookingService(
            IBookingRepository bookingRepository,
            IRoomRepository roomRepository,
            IMapper mapper,
            IValidator<BookingCreateDto> createValidator,
            IValidator<BookingUpdateDto> updateValidator,
            IValidator<BookingQuery> queryValidator)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _mapper = mapper;

            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _queryValidator = queryValidator;
        }

        public async Task<BookingReadDto>
            GetUserBookingByIdAsync(
                string id,
                string userId)
        {
            Booking? booking =
                await _bookingRepository
                    .GetByIdWithDetailsAsync(id);

            if (booking is null)
            {
                throw new NotFoundException(
                    nameof(Booking),
                    id
                );
            }

            EnsureUserBooking(
                booking,
                userId
            );

            return _mapper.Map<BookingReadDto>(
                booking
            );
        }

        public async Task<BookingReadDto>
            GetOwnerBookingByIdAsync(
                string id,
                string ownerId)
        {
            Booking? booking =
                await _bookingRepository
                    .GetByIdWithDetailsAsync(id);

            if (booking is null)
            {
                throw new NotFoundException(
                    nameof(Booking),
                    id
                );
            }

            EnsureOwnerBooking(
                booking,
                ownerId
            );

            return _mapper.Map<BookingReadDto>(
                booking
            );
        }

        public async Task<PagedResult<BookingReadDto>>
            SearchUserBookingsAsync(
                string userId,
                BookingFilterDto filter)
        {
            BookingQuery query =
                CreateQuery(
                    filter,
                    userId: userId
                );

            await _queryValidator
                .ValidateAndThrowAsync(query);

            PagedResult<Booking> result =
                await _bookingRepository
                    .SearchAsync(query);

            return MapPagedResult(
                result
            );
        }

        public async Task<PagedResult<BookingReadDto>>
            SearchOwnerBookingsAsync(
                string ownerId,
                BookingFilterDto filter)
        {
            BookingQuery query =
                CreateQuery(
                    filter,
                    ownerId: ownerId
                );

            await _queryValidator
                .ValidateAndThrowAsync(query);

            PagedResult<Booking> result =
                await _bookingRepository
                    .SearchAsync(query);

            return MapPagedResult(
                result
            );
        }

        public async Task<BookingReadDto> CreateAsync(
            string userId,
            BookingCreateDto dto)
        {
            await _createValidator
                .ValidateAndThrowAsync(dto);

            Room? room =
                await _roomRepository.GetByIdAsync(
                    dto.RoomId
                );

            if (room is null)
            {
                throw new NotFoundException(
                    nameof(Room),
                    dto.RoomId
                );
            }

            if (!room.IsAvailable)
            {
                throw new ConflictException(
                    "Кімната тимчасово недоступна для бронювання."
                );
            }

            DateTime checkIn =
                dto.CheckIn.Date;

            DateTime checkOut =
                dto.CheckOut.Date;

            bool hasConflict =
                await _bookingRepository
                    .HasConflictAsync(
                        room.Id,
                        checkIn,
                        checkOut
                    );

            if (hasConflict)
            {
                throw new BookingConflictException();
            }

            Booking booking =
                _mapper.Map<Booking>(
                    dto
                );

            booking.UserId =
                userId;

            booking.CheckIn =
                checkIn;

            booking.CheckOut =
                checkOut;

            booking.TotalPrice =
                CalculateTotalPrice(
                    room,
                    checkIn,
                    checkOut
                );

            await _bookingRepository.AddAsync(
                booking
            );

            await _bookingRepository
                .SaveChangesAsync();

            return await GetDetailedBookingDtoAsync(
                booking.Id
            );
        }

        public async Task<BookingReadDto> UpdateAsync(
            string id,
            string userId,
            BookingUpdateDto dto)
        {
            await _updateValidator
                .ValidateAndThrowAsync(dto);

            Booking? booking =
                await _bookingRepository.GetByIdAsync(
                    id
                );

            if (booking is null)
            {
                throw new NotFoundException(
                    nameof(Booking),
                    id
                );
            }

            EnsureUserBooking(
                booking,
                userId
            );

            Room? room =
                await _roomRepository.GetByIdAsync(
                    booking.RoomId
                );

            if (room is null)
            {
                throw new NotFoundException(
                    nameof(Room),
                    booking.RoomId
                );
            }

            if (!room.IsAvailable)
            {
                throw new ConflictException(
                    "Кімната тимчасово недоступна для бронювання."
                );
            }

            DateTime checkIn =
                dto.CheckIn?.Date ??
                booking.CheckIn.Date;

            DateTime checkOut =
                dto.CheckOut?.Date ??
                booking.CheckOut.Date;

            BookingCreateDto effectiveBooking =
                new BookingCreateDto
                {
                    RoomId =
                        booking.RoomId,

                    CheckIn =
                        checkIn,

                    CheckOut =
                        checkOut
                };

            await _createValidator
                .ValidateAndThrowAsync(
                    effectiveBooking
                );

            bool hasConflict =
                await _bookingRepository
                    .HasConflictAsync(
                        room.Id,
                        checkIn,
                        checkOut,
                        booking.Id
                    );

            if (hasConflict)
            {
                throw new BookingConflictException();
            }

            _mapper.Map(
                dto,
                booking
            );

            booking.CheckIn =
                checkIn;

            booking.CheckOut =
                checkOut;

            booking.TotalPrice =
                CalculateTotalPrice(
                    room,
                    checkIn,
                    checkOut
                );

            _bookingRepository.Update(
                booking
            );

            await _bookingRepository
                .SaveChangesAsync();

            return await GetDetailedBookingDtoAsync(
                booking.Id
            );
        }

        public async Task DeleteAsync(
            string id,
            string userId)
        {
            Booking? booking =
                await _bookingRepository.GetByIdAsync(
                    id
                );

            if (booking is null)
            {
                throw new NotFoundException(
                    nameof(Booking),
                    id
                );
            }

            EnsureUserBooking(
                booking,
                userId
            );

            _bookingRepository.Delete(
                booking
            );

            await _bookingRepository
                .SaveChangesAsync();
        }

        private async Task<BookingReadDto>
            GetDetailedBookingDtoAsync(
                string id)
        {
            Booking? booking =
                await _bookingRepository
                    .GetByIdWithDetailsAsync(
                        id
                    );

            if (booking is null)
            {
                throw new NotFoundException(
                    nameof(Booking),
                    id
                );
            }

            return _mapper.Map<BookingReadDto>(
                booking
            );
        }

        private static BookingQuery CreateQuery(
            BookingFilterDto filter,
            string? userId = null,
            string? ownerId = null)
        {
            return new BookingQuery
            {
                RoomId =
                    filter.RoomId,

                HotelId =
                    filter.HotelId,

                From =
                    filter.From,

                To =
                    filter.To,

                SortBy =
                    filter.SortBy,

                Descending =
                    filter.Descending,

                Page =
                    filter.Page,

                PageSize =
                    filter.PageSize,

                UserId =
                    userId,

                OwnerId =
                    ownerId
            };
        }

        private PagedResult<BookingReadDto>
            MapPagedResult(
                PagedResult<Booking> result)
        {
            return new PagedResult<BookingReadDto>
            {
                Items =
                    _mapper.Map<
                        IReadOnlyList<BookingReadDto>
                    >(result.Items),

                Page =
                    result.Page,

                PageSize =
                    result.PageSize,

                TotalCount =
                    result.TotalCount
            };
        }

        private static void EnsureUserBooking(
            Booking booking,
            string userId)
        {
            if (booking.UserId != userId)
            {
                throw new ForbiddenOperationException(
                    "Ви не можете керувати чужим бронюванням."
                );
            }
        }

        private static void EnsureOwnerBooking(
            Booking booking,
            string ownerId)
        {
            if (booking.Room.Hotel.OwnerId != ownerId)
            {
                throw new ForbiddenOperationException(
                    "Ви не можете переглядати бронювання чужого готелю."
                );
            }
        }

        private static decimal CalculateTotalPrice(
            Room room,
            DateTime checkIn,
            DateTime checkOut)
        {
            int nights =
                (checkOut.Date -
                 checkIn.Date).Days;

            return room.CostPerNight *
                   nights;
        }
    }
}