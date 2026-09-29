using AutoMapper;
using lab_01.DTOs.User;
using lab_01.Models.Entities;

namespace lab_01.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserReadDto>();
        }
    }
}