using lab_01.Common.Pagination;
using lab_01.Data;
using lab_01.Models.Entities;
using lab_01.Repositories.Interfaces;
using lab_01.Repositories.Queries;
using Microsoft.EntityFrameworkCore;

namespace lab_01.Repositories.Implementations
{
    public class BookingRepository
        : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<Booking?> GetByIdWithDetailsAsync(
            string id)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Include(b => b.User)
                .Include(b => b.Room)
                    .ThenInclude(r => r.Hotel)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<PagedResult<Booking>> SearchAsync(
            BookingQuery query)
        {
            IQueryable<Booking> bookings = _context.Bookings
                .AsNoTracking()
                .Include(b => b.Room)
                    .ThenInclude(r => r.Hotel);

            if (!string.IsNullOrWhiteSpace(query.UserId))
            {
                bookings = bookings.Where(
                    b => b.UserId == query.UserId
                );
            }

            if (!string.IsNullOrWhiteSpace(query.RoomId))
            {
                bookings = bookings.Where(
                    b => b.RoomId == query.RoomId
                );
            }

            if (!string.IsNullOrWhiteSpace(query.HotelId))
            {
                bookings = bookings.Where(
                    b => b.Room.HotelId == query.HotelId
                );
            }

            if (!string.IsNullOrWhiteSpace(query.OwnerId))
            {
                bookings = bookings.Where(
                    b => b.Room.Hotel.OwnerId == query.OwnerId
                );
            }

            if (query.From.HasValue)
            {
                bookings = bookings.Where(
                    b => b.CheckOut >= query.From.Value
                );
            }

            if (query.To.HasValue)
            {
                bookings = bookings.Where(
                    b => b.CheckIn <= query.To.Value
                );
            }

            int totalCount = await bookings.CountAsync();

            bookings = ApplySorting(bookings, query);

            List<Booking> items = await bookings
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<Booking>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> HasConflictAsync(
            string roomId,
            DateTime checkIn,
            DateTime checkOut,
            string? excludeBookingId = null)
        {
            return await _context.Bookings.AnyAsync(
                booking =>
                    booking.RoomId == roomId &&
                    booking.CheckIn < checkOut &&
                    booking.CheckOut > checkIn &&
                    (
                        excludeBookingId == null ||
                        booking.Id != excludeBookingId
                    )
            );
        }

        private static IQueryable<Booking> ApplySorting(
            IQueryable<Booking> bookings,
            BookingQuery query)
        {
            return query.SortBy?.ToLower() switch
            {
                "checkin" => query.Descending
                    ? bookings.OrderByDescending(b => b.CheckIn)
                    : bookings.OrderBy(b => b.CheckIn),

                "checkout" => query.Descending
                    ? bookings.OrderByDescending(b => b.CheckOut)
                    : bookings.OrderBy(b => b.CheckOut),

                "price" => query.Descending
                    ? bookings.OrderByDescending(b => b.TotalPrice)
                    : bookings.OrderBy(b => b.TotalPrice),

                "createdat" => query.Descending
                    ? bookings.OrderByDescending(b => b.CreatedAt)
                    : bookings.OrderBy(b => b.CreatedAt),

                _ => bookings.OrderByDescending(b => b.CreatedAt)
            };
        }
    }
}
