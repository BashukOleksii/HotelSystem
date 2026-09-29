using lab_01.Enums;
using lab_01.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace lab_01.Data
{
    public static class DbInitializer
    {
        private const string AdminRole = "Admin";
        private const string OwnerRole = "Owner";
        private const string UserRole = "User";


        public static async Task InitializeAsync(
            IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManager =
                serviceProvider.GetRequiredService<
                    RoleManager<IdentityRole>
                >();

            UserManager<User> userManager =
                serviceProvider.GetRequiredService<
                    UserManager<User>
                >();

            AppDbContext context =
                serviceProvider.GetRequiredService<
                    AppDbContext
                >();


            await context.Database.MigrateAsync();


            await SeedRolesAsync(
                roleManager
            );


            // =========================
            // Admin
            // =========================

            await SeedUserAsync(
                userManager,
                userName: "admin",
                email: "admin@hotel.com",
                password: "Admin777!",
                role: AdminRole,
                phoneNumber: "+380670000001"
            );


            // =========================
            // Owners
            // =========================

            User owner1 =
                await SeedUserAsync(
                    userManager,
                    userName: "owner1",
                    email: "owner1@hotel.com",
                    password: "Owner777!",
                    role: OwnerRole,
                    phoneNumber: "+380670000002"
                );

            User owner2 =
                await SeedUserAsync(
                    userManager,
                    userName: "owner2",
                    email: "owner2@hotel.com",
                    password: "Owner777!",
                    role: OwnerRole,
                    phoneNumber: "+380670000003"
                );


            // =========================
            // Regular users
            // =========================

            User user1 =
                await SeedUserAsync(
                    userManager,
                    userName: "oleksii",
                    email: "oleksii@gmail.com",
                    password: "User777!",
                    role: UserRole,
                    phoneNumber: "+380670000004"
                );

            User user2 =
                await SeedUserAsync(
                    userManager,
                    userName: "anna",
                    email: "anna@gmail.com",
                    password: "User777!",
                    role: UserRole,
                    phoneNumber: "+380670000005"
                );

            User user3 =
                await SeedUserAsync(
                    userManager,
                    userName: "ivan",
                    email: "ivan@gmail.com",
                    password: "User777!",
                    role: UserRole,
                    phoneNumber: "+380670000006"
                );

            User user4 =
                await SeedUserAsync(
                    userManager,
                    userName: "maria",
                    email: "maria@gmail.com",
                    password: "User777!",
                    role: UserRole,
                    phoneNumber: "+380670000007"
                );


            await SeedDomainDataAsync(
                context,
                owner1,
                owner2,
                user1,
                user2,
                user3,
                user4
            );
        }


        // =====================================================
        // Roles
        // =====================================================

        private static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            [
                AdminRole,
                OwnerRole,
                UserRole
            ];

            foreach (string role in roles)
            {
                if (await roleManager.RoleExistsAsync(
                    role
                ))
                {
                    continue;
                }

                IdentityResult result =
                    await roleManager.CreateAsync(
                        new IdentityRole(role)
                    );

                EnsureSuccess(result);
            }
        }


        // =====================================================
        // Users
        // =====================================================

        private static async Task<User> SeedUserAsync(
            UserManager<User> userManager,
            string userName,
            string email,
            string password,
            string role,
            string? phoneNumber = null)
        {
            User? user =
                await userManager.FindByEmailAsync(
                    email
                );


            if (user is null)
            {
                user = new User
                {
                    UserName = userName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    EmailConfirmed = true
                };


                IdentityResult createResult =
                    await userManager.CreateAsync(
                        user,
                        password
                    );

                EnsureSuccess(
                    createResult
                );
            }


            if (!await userManager.IsInRoleAsync(
                user,
                role
            ))
            {
                IdentityResult roleResult =
                    await userManager.AddToRoleAsync(
                        user,
                        role
                    );

                EnsureSuccess(
                    roleResult
                );
            }


            return user;
        }


        // =====================================================
        // Domain data
        // =====================================================

        private static async Task SeedDomainDataAsync(
            AppDbContext context,
            User owner1,
            User owner2,
            User user1,
            User user2,
            User user3,
            User user4)
        {
            // Не додаємо Hotels повторно при кожному запуску.
            if (await context.Hotels.AnyAsync())
            {
                return;
            }


            // =================================================
            // Hotels
            // =================================================

            Hotel kyivHotel = new Hotel
            {
                OwnerId = owner1.Id,

                Name = "Kyiv Grand Hotel",

                Description =
                    "Сучасний готель у центрі Києва поруч із головними пам'ятками міста.",

                Address = new Address
                {
                    City = "Київ",
                    Street = "Хрещатик",
                    Number = "22"
                }
            };


            Hotel lvivHotel = new Hotel
            {
                OwnerId = owner1.Id,

                Name = "Lviv Central Hotel",

                Description =
                    "Затишний готель у центральній частині Львова неподалік площі Ринок.",

                Address = new Address
                {
                    City = "Львів",
                    Street = "Городоцька",
                    Number = "15"
                }
            };


            Hotel odesaHotel = new Hotel
            {
                OwnerId = owner2.Id,

                Name = "Odesa Sea View",

                Description =
                    "Готель біля моря з комфортними номерами та видом на узбережжя.",

                Address = new Address
                {
                    City = "Одеса",
                    Street = "Аркадійська алея",
                    Number = "7"
                }
            };


            Hotel bukovelHotel = new Hotel
            {
                OwnerId = owner2.Id,

                Name = "Bukovel Mountain Resort",

                Description =
                    "Гірський готель для активного зимового та літнього відпочинку.",

                Address = new Address
                {
                    City = "Буковель",
                    Street = "Урочище Вишня",
                    Number = "160"
                }
            };


            Hotel[] hotels =
            [
                kyivHotel,
                lvivHotel,
                odesaHotel,
                bukovelHotel
            ];


            await context.Hotels.AddRangeAsync(
                hotels
            );

            await context.SaveChangesAsync();


            // =================================================
            // Rooms
            // =================================================

            List<Room> rooms = [];


            // -------------------------
            // Kyiv
            // -------------------------

            Room kyiv101 = CreateRoom(
                kyivHotel,
                "101",
                RoomType.Single,
                1200m,
                1
            );

            Room kyiv102 = CreateRoom(
                kyivHotel,
                "102",
                RoomType.Double,
                1800m,
                2
            );

            Room kyiv201 = CreateRoom(
                kyivHotel,
                "201",
                RoomType.Twin,
                2000m,
                2
            );

            Room kyiv301 = CreateRoom(
                kyivHotel,
                "301",
                RoomType.Suite,
                3500m,
                3
            );

            Room kyiv401 = CreateRoom(
                kyivHotel,
                "401",
                RoomType.Family,
                4200m,
                5,
                isAvailable: false
            );


            rooms.AddRange(
            [
                kyiv101,
                kyiv102,
                kyiv201,
                kyiv301,
                kyiv401
            ]);


            // -------------------------
            // Lviv
            // -------------------------

            Room lviv101 = CreateRoom(
                lvivHotel,
                "101",
                RoomType.Single,
                1000m,
                1
            );

            Room lviv102 = CreateRoom(
                lvivHotel,
                "102",
                RoomType.Double,
                1500m,
                2
            );

            Room lviv201 = CreateRoom(
                lvivHotel,
                "201",
                RoomType.Twin,
                1700m,
                2
            );

            Room lviv301 = CreateRoom(
                lvivHotel,
                "301",
                RoomType.Suite,
                2800m,
                3
            );

            Room lviv401 = CreateRoom(
                lvivHotel,
                "401",
                RoomType.Family,
                3300m,
                4
            );


            rooms.AddRange(
            [
                lviv101,
                lviv102,
                lviv201,
                lviv301,
                lviv401
            ]);


            // -------------------------
            // Odesa
            // -------------------------

            Room odesa101 = CreateRoom(
                odesaHotel,
                "101",
                RoomType.Single,
                1400m,
                1
            );

            Room odesa102 = CreateRoom(
                odesaHotel,
                "102",
                RoomType.Double,
                2100m,
                2
            );

            Room odesa201 = CreateRoom(
                odesaHotel,
                "201",
                RoomType.Twin,
                2200m,
                2
            );

            Room odesa301 = CreateRoom(
                odesaHotel,
                "301",
                RoomType.Suite,
                4000m,
                3
            );

            Room odesa401 = CreateRoom(
                odesaHotel,
                "401",
                RoomType.Family,
                4800m,
                5
            );


            rooms.AddRange(
            [
                odesa101,
                odesa102,
                odesa201,
                odesa301,
                odesa401
            ]);


            // -------------------------
            // Bukovel
            // -------------------------

            Room bukovel101 = CreateRoom(
                bukovelHotel,
                "101",
                RoomType.Single,
                1600m,
                1
            );

            Room bukovel102 = CreateRoom(
                bukovelHotel,
                "102",
                RoomType.Double,
                2400m,
                2
            );

            Room bukovel201 = CreateRoom(
                bukovelHotel,
                "201",
                RoomType.Twin,
                2500m,
                2
            );

            Room bukovel301 = CreateRoom(
                bukovelHotel,
                "301",
                RoomType.Suite,
                4500m,
                3
            );

            Room bukovel401 = CreateRoom(
                bukovelHotel,
                "401",
                RoomType.Family,
                5200m,
                6,
                isAvailable: false
            );


            rooms.AddRange(
            [
                bukovel101,
                bukovel102,
                bukovel201,
                bukovel301,
                bukovel401
            ]);


            await context.Rooms.AddRangeAsync(
                rooms
            );

            await context.SaveChangesAsync();


            // =================================================
            // Bookings
            // =================================================

            DateTime today =
                DateTime.UtcNow.Date;


            Booking[] bookings =
            [
                // Минуле бронювання.
                CreateBooking(
                    user1,
                    kyiv102,
                    today.AddDays(-20),
                    nights: 3
                ),

                // Майбутнє бронювання.
                CreateBooking(
                    user1,
                    lviv301,
                    today.AddDays(10),
                    nights: 4
                ),

                CreateBooking(
                    user2,
                    kyiv301,
                    today.AddDays(5),
                    nights: 2
                ),

                CreateBooking(
                    user2,
                    odesa201,
                    today.AddDays(15),
                    nights: 5
                ),

                CreateBooking(
                    user3,
                    odesa102,
                    today.AddDays(2),
                    nights: 3
                ),

                CreateBooking(
                    user3,
                    bukovel301,
                    today.AddDays(20),
                    nights: 5
                ),

                CreateBooking(
                    user4,
                    lviv201,
                    today.AddDays(-12),
                    nights: 2
                ),

                CreateBooking(
                    user4,
                    bukovel102,
                    today.AddDays(7),
                    nights: 3
                )
            ];


            await context.Bookings.AddRangeAsync(
                bookings
            );

            await context.SaveChangesAsync();


            // =================================================
            // Reviews
            // =================================================

            Review[] reviews =
            [
                CreateReview(
                    user1,
                    kyivHotel,
                    5,
                    "Чудове розташування та дуже привітний персонал."
                ),

                CreateReview(
                    user2,
                    kyivHotel,
                    4,
                    "Хороший готель у самому центрі міста."
                ),

                CreateReview(
                    user3,
                    kyivHotel,
                    5,
                    "Все сподобалося, номер був чистий та комфортний."
                ),


                CreateReview(
                    user1,
                    lvivHotel,
                    5,
                    "Дуже затишний готель та зручне розташування."
                ),

                CreateReview(
                    user4,
                    lvivHotel,
                    4,
                    "Гарний варіант для відпочинку у Львові."
                ),


                CreateReview(
                    user2,
                    odesaHotel,
                    4,
                    "Гарний вид і комфортний номер."
                ),

                CreateReview(
                    user3,
                    odesaHotel,
                    5,
                    "Чудове місце для літнього відпочинку."
                ),


                CreateReview(
                    user3,
                    bukovelHotel,
                    5,
                    "Ідеальний варіант для відпочинку в горах."
                ),

                CreateReview(
                    user4,
                    bukovelHotel,
                    4,
                    "Все добре, особливо сподобалося розташування."
                )
            ];


            await context.Reviews.AddRangeAsync(
                reviews
            );

            await context.SaveChangesAsync();
        }


        // =====================================================
        // Factory helpers
        // =====================================================

        private static Room CreateRoom(
            Hotel hotel,
            string roomNumber,
            RoomType type,
            decimal costPerNight,
            int capacity,
            bool isAvailable = true)
        {
            return new Room
            {
                HotelId = hotel.Id,

                RoomNumber = roomNumber,

                Type = type,

                CostPerNight = costPerNight,

                Capacity = capacity,

                IsAvailable = isAvailable
            };
        }


        private static Booking CreateBooking(
            User user,
            Room room,
            DateTime checkIn,
            int nights)
        {
            DateTime checkOut =
                checkIn.AddDays(nights);

            return new Booking
            {
                UserId = user.Id,

                RoomId = room.Id,

                CheckIn = checkIn,

                CheckOut = checkOut,

                TotalPrice =
                    room.CostPerNight * nights
            };
        }


        private static Review CreateReview(
            User user,
            Hotel hotel,
            int rating,
            string? comment)
        {
            return new Review
            {
                UserId = user.Id,

                HotelId = hotel.Id,

                Rating = rating,

                Comment = comment
            };
        }


        // =====================================================
        // Identity helper
        // =====================================================

        private static void EnsureSuccess(
            IdentityResult result)
        {
            if (result.Succeeded)
            {
                return;
            }

            string errors =
                string.Join(
                    ", ",
                    result.Errors.Select(
                        error => error.Description
                    )
                );

            throw new Exception(
                errors
            );
        }
    }
}