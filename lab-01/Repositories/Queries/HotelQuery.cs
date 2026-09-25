using lab_01.Common.Queries;

namespace lab_01.Repositories.Queries
{
    public class HotelQuery : BaseQuery
    {
        public string? OwnerId { get; set; }
        public string? City { get; set; }
    }
}
