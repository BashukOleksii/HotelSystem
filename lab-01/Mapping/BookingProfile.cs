using AutoMapper;
using lab_01.DTOs.Booking;
using lab_01.Models.Entities;

namespace lab_01.Mapping
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<BookingCreateDto, Booking>();
            CreateMap<Booking, BookingReadDto>();
            CreateMap<BookingUpdateDto, Booking>()
                .ForAllMembers(option =>
                {
                    option.Condition((src, dest, srcMember) => srcMember != null);
                });
        }
    }
}
