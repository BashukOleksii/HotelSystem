using AutoMapper;
using lab_01.DTOs.Room;
using lab_01.Models.Entities;

namespace lab_01.Mapping
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<RoomCreateDto, Room>();
            CreateMap<Room, RoomReadDto>();
            CreateMap<RoomUpdateDto, Room>()
                .ForAllMembers(option =>
                {
                    option.Condition((src, dest, srcMember) => srcMember != null);
                });
        }
    }
}
