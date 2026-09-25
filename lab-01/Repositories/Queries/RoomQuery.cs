using lab_01.Common.Queries;
using lab_01.Enums;

namespace lab_01.Repositories.Queries
{
    public class RoomQuery : BaseQuery
    {
        public string? HotelId { get; set; }
        public RoomType? Type { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinCapacity { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
    }
}
