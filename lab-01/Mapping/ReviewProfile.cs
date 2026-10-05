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

            CreateMap<Review, ReviewReadDto>()
                .ForMember(
                    destination => destination.HotelName,
                    options => options.MapFrom(
                        source => source.Hotel.Name
                    )
                )
                .ForMember(
                    destination => destination.UserEmail,
                    options => options.MapFrom(
                        source => source.User != null
                            ? source.User.Email
                            : null
                    )
                );

            CreateMap<ReviewUpdateDto, Review>()
                .ForAllMembers(options =>
                {
                    options.Condition(
                        (source, destination, sourceMember) =>
                            sourceMember != null
                    );
                });
        }
    }
}