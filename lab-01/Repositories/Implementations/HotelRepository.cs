using lab_01.Common.Pagination;
using lab_01.Data;
using lab_01.Models.Entities;
using lab_01.Repositories.Interfaces;
using lab_01.Repositories.Queries;
using Microsoft.EntityFrameworkCore;

namespace lab_01.Repositories.Implementations
{

        public class HotelRepository
            : Repository<Hotel>, IHotelRepository
        {
            public HotelRepository(AppDbContext context)
                : base(context)
            {
            }

            public async Task<Hotel?> GetByIdWithDetailsAsync(string id)
            {
                return await _context.Hotels
                    .AsNoTracking()
                    .Include(h => h.Rooms)
                    .Include(h => h.Reviews)
                    .FirstOrDefaultAsync(h => h.Id == id);
            }

        public async Task<PagedResult<Hotel>> SearchAsync(
                HotelQuery query)
            {
                IQueryable<Hotel> hotels = _context.Hotels
                    .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(query.Search))
                {
                    string search = query.Search.Trim();

                    hotels = hotels.Where(h =>
                        EF.Functions.Like(
                            h.Name,
                            $"%{search}%"
                        )
                        ||
                        (
                            h.Description != null &&
                            EF.Functions.Like(
                                h.Description,
                                $"%{search}%"
                            )
                        )
                    );
                }

                if (!string.IsNullOrWhiteSpace(query.OwnerId))
                {
                    hotels = hotels.Where(
                        h => h.OwnerId == query.OwnerId
                    );
                }

                if (!string.IsNullOrWhiteSpace(query.City))
                {
                    hotels = hotels.Where(
                        h => h.Address.City == query.City
                    );
                }

                int totalCount = await hotels.CountAsync();

                hotels = ApplySorting(hotels, query);

                List<Hotel> items = await hotels
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync();

                return new PagedResult<Hotel>
                {
                    Items = items,
                    Page = query.Page,
                    PageSize = query.PageSize,
                    TotalCount = totalCount
                };
            }

            private static IQueryable<Hotel> ApplySorting(
                IQueryable<Hotel> hotels,
                HotelQuery query)
            {
                return query.SortBy?.ToLower() switch
                {
                    "name" => query.Descending
                        ? hotels.OrderByDescending(h => h.Name)
                        : hotels.OrderBy(h => h.Name),

                    "city" => query.Descending
                        ? hotels.OrderByDescending(h => h.Address.City)
                        : hotels.OrderBy(h => h.Address.City),

                    "createdat" => query.Descending
                        ? hotels.OrderByDescending(h => h.CreatedAt)
                        : hotels.OrderBy(h => h.CreatedAt),

                    _ => hotels.OrderBy(h => h.Name)
                };
            }
        }
    
}
