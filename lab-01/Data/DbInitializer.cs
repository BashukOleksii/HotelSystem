using lab_01.Models.Entities;
using lab_01.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace lab_01.Data
{
    public static class DbInitializer
    {
        private static readonly string[] roles = ["Admin", "User", "Owner"];

        private static void EnsureSuccess(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {


            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));

                    EnsureSuccess(result);
                }

            }

        }

        private static async Task<User> SeedUserAsync(
            UserManager<User> userManager,
            string userEmail,
            string userPassword,
            string userRole)
        {
            var user = await userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                user = new User
                {
                    Email = userEmail,
                    UserName = userEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, userPassword);
                EnsureSuccess(result);
            }

            if (!await userManager.IsInRoleAsync(user, userRole))
            {
                var result = await userManager.AddToRoleAsync(user, userRole);
                EnsureSuccess(result);
            }

            return user;
        }

        private static async Task SeedDomainData(
            AppDbContext appDbContext,
            User owner,
            User user,
            User admin
        )
        {
            if (await appDbContext.Hotels.AnyAsync())
                return;

            Hotel hotel = new Hotel
            {
                OwnerId = owner.Id,
                Name = "Головний готель",
                Description = "Головний готель для тестування",
                Address = new Address
                {
                    City = "Київ",
                    Street = "Хрещатик",
                    Number = "10a"
                }
            };

            appDbContext.Hotels.Add(hotel);
            await appDbContext.SaveChangesAsync();


            Room[] rooms = new Room[5];
            int roomIndex = 4;

            for(int i = 0; i < 5; i++)
            {
                bool available = true;

                if(i == roomIndex)
                {
                    available = false;
                }

                rooms[i] = new Room
                {
                    HotelId = hotel.Id,
                    RoomNumber = (i + 1).ToString(),
                    Type = (RoomType)(i % 3),
                    CostPerNight = 100 + i * 50,
                    Capacity = 2 + i,
                    IsAvailable = available,
                };
            }

            appDbContext.Rooms.AddRange(rooms);
            await appDbContext.SaveChangesAsync();



            Booking booking = new Booking
            {
                UserId = user.Id,
                RoomId = rooms[roomIndex].Id,
                CheckIn = DateTime.Now.AddDays(1)
            };

            appDbContext.Bookings.Add(booking);
            await appDbContext.SaveChangesAsync();

            Review review = new Review
            {
                UserId = user.Id,
                HotelId = hotel.Id,
                Rating = 5,
                Comment = "Чудовий готель!"
            };

            appDbContext.Reviews.Add(review);
            await appDbContext.SaveChangesAsync();

        }

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                await appDbContext.Database.MigrateAsync();

                await SeedRolesAsync(roleManager);
                var admin = await SeedUserAsync(userManager, "Admin@email.com", "Admin777!", roles[0]);
                var user = await SeedUserAsync(userManager, "User@email.com", "User777!", roles[1]);
                var owner = await SeedUserAsync(userManager, "Owner@email.com", "Owner777!", roles[2]);

                await SeedDomainData(appDbContext, owner, user, admin);
            }
        }
    }
}
