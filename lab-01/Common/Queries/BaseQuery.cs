namespace lab_01.Common.Queries
{
    public abstract class BaseQuery
    {
        public string? Search { get; set; }

        public string? SortBy { get; set; }

        public bool Descending { get; set; } = false;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
