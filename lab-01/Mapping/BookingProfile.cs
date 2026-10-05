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
            CreateMap<Booking, BookingReadDto>()
            .ForMember(
                dest => dest.UserEmail,
                opt => opt.MapFrom(
                    src => src.User != null
                        ? src.User.Email
                        : null
                )
            )
            .ForMember(
                dest => dest.RoomNumber,
                opt => opt.MapFrom(
                    src => src.Room.RoomNumber
                )
            )
            .ForMember(
                dest => dest.HotelId,
                opt => opt.MapFrom(
                    src => src.Room.HotelId
                )
            )
            .ForMember(
                dest => dest.HotelName,
                opt => opt.MapFrom(
                    src => src.Room.Hotel.Name
                )
            );
            CreateMap<BookingUpdateDto, Booking>()
                .ForAllMembers(option =>
                {
                    option.Condition((src, dest, srcMember) => srcMember != null);
                });
        }
    }
}
