using lab_01.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace lab_01.Data
{
    public class AppDbContext
        : IdentityDbContext<User>
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<HotelPhoto> HotelPhotos { get; set; }

        public DbSet<ReviewPhoto> ReviewPhotos { get; set; }

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Booking>()
                .Property(booking =>
                    booking.TotalPrice)
                .HasPrecision(18, 2);

            builder.Entity<Room>()
                .Property(room =>
                    room.CostPerNight)
                .HasPrecision(18, 2);

            builder.Entity<Hotel>()
                .OwnsOne(hotel =>
                    hotel.Address);

            builder.Entity<Hotel>()
                .Navigation(hotel =>
                    hotel.Address)
                .IsRequired();

            builder.Entity<HotelPhoto>()
                .Property(photo =>
                    photo.Url)
                .HasMaxLength(500)
                .IsRequired();

            builder.Entity<HotelPhoto>()
                .HasOne(photo =>
                    photo.Hotel)
                .WithMany(hotel =>
                    hotel.Photos)
                .HasForeignKey(photo =>
                    photo.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ReviewPhoto>()
                .Property(photo =>
                    photo.Url)
                .HasMaxLength(500)
                .IsRequired();

            builder.Entity<ReviewPhoto>()
                .HasOne(photo =>
                    photo.Review)
                .WithMany(review =>
                    review.Photos)
                .HasForeignKey(photo =>
                    photo.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}