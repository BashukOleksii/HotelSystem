using AutoMapper;
using lab_01.DTOs.Review;
using lab_01.Models.Entities;

namespace lab_01.Mapping
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<ReviewCreateDto, Review>();
            CreateMap<Review, ReviewReadDto>();
            CreateMap<ReviewUpdateDto, Review>()
                .ForAllMembers(option =>
                {
                    option.Condition((src, dest, srcMember) => srcMember != null);
                });
        }
}
