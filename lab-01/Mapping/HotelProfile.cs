using AutoMapper;
using lab_01.DTOs.Hotel;
using lab_01.Models.Entities;

namespace lab_01.Mapping
{
    public class HotelProfile : Profile
    {
        public HotelProfile()
        {
            CreateMap<HotelCreateDto, Hotel>();
            CreateMap<HotelCreateDto, Hotel>();
            CreateMap<HotelUpdateDto, Hotel>()
                .ForAllMembers(option =>
                {
                    option.Condition((src, dest, srcMember) => srcMember != null);
                });
        }
    }
}
