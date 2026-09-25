using AutoMapper;
using lab_01.DTOs.Hotel.Address;
using lab_01.Models.Entities;

namespace lab_01.Mapping
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<Address, AddressDto>().ReverseMap();
        }
    }
}
