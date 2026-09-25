using lab_01.Common.Pagination;
using lab_01.Models.Entities;
using lab_01.Repositories.Queries;

namespace lab_01.Repositories.Interfaces
{
    public interface IHotelRepository : IRepository<Hotel>
    {
        Task<Hotel?> GetByIdWithDetailsAsync(string id);
        Task<PagedResult<Hotel>> SearchAsync(HotelQuery query);
    }
}
