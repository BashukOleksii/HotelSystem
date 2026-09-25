using lab_01.DTOs.Base;
using lab_01.DTOs.Hotel.Address;
using lab_01.Models.Entities;

namespace lab_01.DTOs.Hotel
{
    public class HotelReadDto : BaseReadDto
    {
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public AddressDto Address { get; set; }
    }
}
