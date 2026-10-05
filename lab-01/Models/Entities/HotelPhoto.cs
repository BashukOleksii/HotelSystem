using lab_01.Models.Entities.Base;

namespace lab_01.Models.Entities
{
    public class HotelPhoto : BaseEntry
    {
        public string HotelId { get; set; }

        public string Url { get; set; }

        public Hotel Hotel { get; set; }
    }
}