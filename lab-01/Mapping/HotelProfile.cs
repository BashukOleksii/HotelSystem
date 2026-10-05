using AutoMapper;
using lab_01.DTOs.Hotel;
using lab_01.Models.Entities;

namespace lab_01.Mapping
{
    public class HotelProfile : Profile
    {
        public HotelProfile()
        {
            CreateMap<HotelCreateDto, Hotel>()
                .ForMember(
                    destination =>
                        destination.Photos,
                    options =>
                        options.Ignore()
                );

            CreateMap<Hotel, HotelReadDto>()
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

            CreateMap<HotelUpdateDto, Hotel>()
                .ForMember(
                    destination =>
                        destination.Photos,
                    options =>
                        options.Ignore()
                )
                .ForAllMembers(option =>
                {
                    option.Condition(
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