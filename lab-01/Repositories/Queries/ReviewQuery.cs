using lab_01.Common.Queries;

namespace lab_01.Repositories.Queries
{
    public class ReviewQuery : BaseQuery
    {
        public string? HotelId { get; set; }
        public string? UserId { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
    }
}
