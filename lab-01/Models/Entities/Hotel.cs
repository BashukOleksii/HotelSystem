using lab_01.Models.Entities.Base;

namespace lab_01.Models.Entities
{
    public class Hotel : BaseEntry
    {
        public string OwnerId { get; set; }
        public string Name { get; set; } 
        public string? Description { get; set; }
        public Address Address { get; set; }

        public User Owner { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
