using lab_01.Models.Entities.Base;

namespace lab_01.Models.Entities
{
    public class ReviewPhoto : BaseEntry
    {
        public string ReviewId { get; set; }

        public string Url { get; set; }

        public Review Review { get; set; }
    }
}