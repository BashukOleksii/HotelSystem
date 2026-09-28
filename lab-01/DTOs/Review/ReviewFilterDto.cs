namespace lab_01.DTOs.Review
{
    public class ReviewFilterDto
    {
        public string? Search { get; set; }

        public string? HotelId { get; set; }

        public int? MinRating { get; set; }

        public int? MaxRating { get; set; }

        public string? SortBy { get; set; }

        public bool Descending { get; set; } = false;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}