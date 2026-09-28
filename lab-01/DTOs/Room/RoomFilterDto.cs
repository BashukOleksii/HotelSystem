using lab_01.Enums;

namespace lab_01.DTOs.Room
{
    public class RoomFilterDto
    {
        public string? Search { get; set; }

        public string? HotelId { get; set; }

        public RoomType? Type { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public int? MinCapacity { get; set; }

        public bool? IsAvailable { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public string? SortBy { get; set; }

        public bool Descending { get; set; } = false;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}