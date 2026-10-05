using AutoMapper;
using lab_01.DTOs.Review;
using lab_01.Models.Entities;

namespace lab_01.Mapping
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<ReviewCreateDto, Review>()
                .ForMember(
                    destination =>
                        destination.Photos,
                    options =>
                        options.Ignore()
                );

            CreateMap<Review, ReviewReadDto>()
                .ForMember(
                    destination =>
                        destination.HotelName,
                    options =>
                        options.MapFrom(
                            source =>
                                source.Hotel.Name
                        )
                )
                .ForMember(
                    destination =>
                        destination.UserEmail,
                    options =>
                        options.MapFrom(
                            source =>
                                source.User != null
                                    ? source.User.Email
                                    : null
                        )
                )
                .ForMember(
                    destination =>
                        destination.PhotoUrls,
                    options =>
                        options.MapFrom(
                            source =>
                                source.Photos
                                    .Select(photo =>
                                        photo.Url)
                                    .ToList()
                        )
                );

            CreateMap<ReviewUpdateDto, Review>()
                .ForMember(
                    destination =>
                        destination.Photos,
                    options =>
                        options.Ignore()
                )
                .ForAllMembers(options =>
                {
                    options.Condition(
                        (
                            source,
                            destination,
                            sourceMember
                        ) =>
                            sourceMember != null
                    );
                });
        }
    }
}