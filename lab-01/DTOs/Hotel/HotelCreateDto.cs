
using lab_01.DTOs.Hotel.Address;

namespace lab_01.DTOs.Hotel
{
    public class HotelCreateDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public AddressDto Address { get; set; }
    }
}
