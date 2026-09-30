using lab_01.Common.Pagination;
using lab_01.DTOs.Hotel;
using lab_01.DTOs.Room;

namespace lab_01.ViewModels.Rooms
{
    public class OwnerRoomIndexViewModel
    {
        public HotelReadDto Hotel { get; set; }

        public RoomFilterDto Filter { get; set; }
            = new();

        public PagedResult<RoomReadDto> Rooms { get; set; }
            = new();
    }
}