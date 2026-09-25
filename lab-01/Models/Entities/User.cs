using Microsoft.AspNetCore.Identity;

namespace lab_01.Models.Entities
{
    public class User : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Hotel> OwnedHotels { get; set; } = new List<Hotel>();
        public ICollection<Booking> Reservations { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
