using lab_01.Common.Queries;

namespace lab_01.Repositories.Queries
{
    public class BookingQuery : BaseQuery
    {
        public string? UserId { get; set; }
        public string? RoomId { get; set; }
        public string? HotelId { get; set; }
        public string? OwnerId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
