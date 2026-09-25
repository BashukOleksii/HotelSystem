using lab_01.Common.Pagination;
using lab_01.Data;
using lab_01.Models.Entities;
using lab_01.Repositories.Implementations.lab_01.Repositories.Implementations;
using lab_01.Repositories.Interfaces;
using lab_01.Repositories.Queries;
using Microsoft.EntityFrameworkCore;

namespace lab_01.Repositories.Implementations
{
    public class RoomRepository
       : Repository<Room>, IRoomRepository
    {
        public RoomRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<Room?> GetByIdWithDetailsAsync(
            string id)
        {
            return await _context.Rooms
                .AsNoTracking()
                .Include(r => r.Hotel)
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<PagedResult<Room>> SearchAsync(
            RoomQuery query)
        {
            IQueryable<Room> rooms = _context.Rooms
                .AsNoTracking()
                .Include(r => r.Hotel);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim();

                rooms = rooms.Where(r =>
                    EF.Functions.Like(
                        r.RoomNumber,
                        $"%{search}%"
                    )
                    ||
                    EF.Functions.Like(
                        r.Hotel.Name,
                        $"%{search}%"
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(query.HotelId))
            {
                rooms = rooms.Where(
                    r => r.HotelId == query.HotelId
                );
            }

            if (query.Type.HasValue)
            {
                rooms = rooms.Where(
                    r => r.Type == query.Type.Value
                );
            }

            if (query.MinPrice.HasValue)
            {
                rooms = rooms.Where(
                    r => r.CostPerNight >= query.MinPrice.Value
                );
            }

            if (query.MaxPrice.HasValue)
            {
                rooms = rooms.Where(
                    r => r.CostPerNight <= query.MaxPrice.Value
                );
            }

            if (query.MinCapacity.HasValue)
            {
                rooms = rooms.Where(
                    r => r.Capacity >= query.MinCapacity.Value
                );
            }

            if (query.IsAvailable.HasValue)
            {
                rooms = rooms.Where(
                    r => r.IsAvailable == query.IsAvailable.Value
                );
            }

            if (query.CheckIn.HasValue &&
                query.CheckOut.HasValue)
            {
                DateTime checkIn = query.CheckIn.Value;
                DateTime checkOut = query.CheckOut.Value;

                rooms = rooms.Where(room =>
                    !room.Bookings.Any(booking =>
                        booking.CheckIn < checkOut &&
                        booking.CheckOut > checkIn
                    )
                );
            }

            int totalCount = await rooms.CountAsync();

            rooms = ApplySorting(rooms, query);

            List<Room> items = await rooms
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<Room>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> ExistsByNumberAsync(
            string hotelId,
            string roomNumber,
            string? excludeRoomId = null)
        {
            return await _context.Rooms.AnyAsync(room =>
                room.HotelId == hotelId &&
                room.RoomNumber == roomNumber &&
                (
                    excludeRoomId == null ||
                    room.Id != excludeRoomId
                )
            );
        }

        public async Task<bool> IsAvailableAsync(
            string roomId,
            DateTime checkIn,
            DateTime checkOut)
        {
            return !await _context.Bookings.AnyAsync(
                booking =>
                    booking.RoomId == roomId &&
                    booking.CheckIn < checkOut &&
                    booking.CheckOut > checkIn
            );
        }

        private static IQueryable<Room> ApplySorting(
            IQueryable<Room> rooms,
            RoomQuery query)
        {
            return query.SortBy?.ToLower() switch
            {
                "number" => query.Descending
                    ? rooms.OrderByDescending(r => r.RoomNumber)
                    : rooms.OrderBy(r => r.RoomNumber),

                "price" => query.Descending
                    ? rooms.OrderByDescending(r => r.CostPerNight)
                    : rooms.OrderBy(r => r.CostPerNight),

                "capacity" => query.Descending
                    ? rooms.OrderByDescending(r => r.Capacity)
                    : rooms.OrderBy(r => r.Capacity),

                "type" => query.Descending
                    ? rooms.OrderByDescending(r => r.Type)
                    : rooms.OrderBy(r => r.Type),

                "createdat" => query.Descending
                    ? rooms.OrderByDescending(r => r.CreatedAt)
                    : rooms.OrderBy(r => r.CreatedAt),

                _ => rooms.OrderBy(r => r.RoomNumber)
            };
        }
    }
}
