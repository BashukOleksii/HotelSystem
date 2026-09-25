using lab_01.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace lab_01.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Booking>()
                 .Property(b => b.TotalPrice)
                 .HasPrecision(18, 2);

            builder.Entity<Room>()
                .Property(r => r.CostPerNight)
                .HasPrecision(18, 2);

            builder.Entity<Hotel>()
                .OwnsOne(h => h.Address);

            builder.Entity<Hotel>()
                .Navigation(h => h.Address)
                .IsRequired();
        }
    }
}

