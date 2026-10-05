using lab_01.Common.Pagination;
using lab_01.Data;
using lab_01.Models.Entities;
using lab_01.Repositories.Interfaces;
using lab_01.Repositories.Queries;
using Microsoft.EntityFrameworkCore;

namespace lab_01.Repositories.Implementations
{
    public class HotelRepository
        : Repository<Hotel>,
          IHotelRepository
    {
        public HotelRepository(
            AppDbContext context)
            : base(context)
        {
        }

        public async Task<Hotel?>
            GetByIdWithDetailsAsync(
                string id)
        {
            return await _context.Hotels
                .AsNoTracking()
                .Include(hotel =>
                    hotel.Photos)
                .Include(hotel =>
                    hotel.Rooms)
                .Include(hotel =>
                    hotel.Reviews)
                .FirstOrDefaultAsync(
                    hotel =>
                        hotel.Id == id
                );
        }

        public async Task<PagedResult<Hotel>>
            SearchAsync(
                HotelQuery query)
        {
            IQueryable<Hotel> hotels =
                _context.Hotels
                    .AsNoTracking()
                    .Include(hotel =>
                        hotel.Photos);

            if (!string.IsNullOrWhiteSpace(
                query.Search))
            {
                string search =
                    query.Search.Trim();

                hotels =
                    hotels.Where(hotel =>
                        EF.Functions.Like(
                            hotel.Name,
                            $"%{search}%"
                        )
                        ||
                        (
                            hotel.Description != null &&
                            EF.Functions.Like(
                                hotel.Description,
                                $"%{search}%"
                            )
                        )
                    );
            }

            if (!string.IsNullOrWhiteSpace(
                query.OwnerId))
            {
                hotels =
                    hotels.Where(
                        hotel =>
                            hotel.OwnerId ==
                            query.OwnerId
                    );
            }

            if (!string.IsNullOrWhiteSpace(
                query.City))
            {
                hotels =
                    hotels.Where(
                        hotel =>
                            hotel.Address.City ==
                            query.City
                    );
            }

            int totalCount =
                await hotels.CountAsync();

            hotels =
                ApplySorting(
                    hotels,
                    query
                );

            List<Hotel> items =
                await hotels
                    .Skip(
                        (query.Page - 1) *
                        query.PageSize
                    )
                    .Take(
                        query.PageSize
                    )
                    .ToListAsync();

            return new PagedResult<Hotel>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        private static IQueryable<Hotel>
            ApplySorting(
                IQueryable<Hotel> hotels,
                HotelQuery query)
        {
            return query.SortBy?.ToLower()
                switch
            {
                "name" =>
                    query.Descending
                        ? hotels.OrderByDescending(
                            hotel =>
                                hotel.Name
                        )
                        : hotels.OrderBy(
                            hotel =>
                                hotel.Name
                        ),

                "city" =>
                    query.Descending
                        ? hotels.OrderByDescending(
                            hotel =>
                                hotel.Address.City
                        )
                        : hotels.OrderBy(
                            hotel =>
                                hotel.Address.City
                        ),

                "createdat" =>
                    query.Descending
                        ? hotels.OrderByDescending(
                            hotel =>
                                hotel.CreatedAt
                        )
                        : hotels.OrderBy(
                            hotel =>
                                hotel.CreatedAt
                        ),

                _ =>
                    hotels.OrderBy(
                        hotel =>
                            hotel.Name
                    )
            };
        }
    }
}